using MediatR;
using Microsoft.AspNetCore.Mvc;
using StripePayment.Application.Commands.Webhooks;
using StripePayment.Application.Interfaces;
using StripePayment.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Text;

namespace StripePayment.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPaymentService _paymentService;
    private readonly StripeOptions _stripeOptions;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(
        IMediator mediator,
        IPaymentService paymentService,
        IOptions<StripeOptions> stripeOptions,
        ILogger<WebhooksController> logger)
    {
        _mediator = mediator;
        _paymentService = paymentService;
        _stripeOptions = stripeOptions.Value;
        _logger = logger;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripeWebhookAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Read the request body
            using var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8);
            var payload = await reader.ReadToEndAsync(cancellationToken);

            // Get the Stripe signature from headers
            var signature = HttpContext.Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Missing Stripe signature header");
                return BadRequest("Missing Stripe signature");
            }

            // Verify webhook signature
            if (!_paymentService.VerifyWebhookSignature(payload, signature, _stripeOptions.WebhookSecret))
            {
                _logger.LogWarning("Invalid Stripe webhook signature");
                return BadRequest("Invalid signature");
            }

            // Parse event type from payload
            var eventData = System.Text.Json.JsonDocument.Parse(payload);
            var eventType = eventData.RootElement.GetProperty("type").GetString();

            if (string.IsNullOrEmpty(eventType))
            {
                _logger.LogWarning("Missing event type in webhook payload");
                return BadRequest("Missing event type");
            }

            _logger.LogInformation("Processing Stripe webhook: {EventType}", eventType);

            // Process the webhook
            var command = new ProcessWebhookCommand(eventType, payload, signature);
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Webhook processing failed: {Error}", result.Error.Message);
                return BadRequest(new { Error = result.Error.Code, Message = result.Error.Message });
            }

            _logger.LogInformation("Webhook processed successfully: {EventType}", eventType);
            return Ok(new { Status = "Success", Message = "Webhook processed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing webhook");
            return StatusCode(500, new { Error = "Webhook.ProcessingError", Message = "An unexpected error occurred" });
        }
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}