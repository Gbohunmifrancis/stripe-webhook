using MediatR;
using Microsoft.AspNetCore.Mvc;
using StripePayment.Application.Commands.Payments;
using StripePayment.Application.DTOs;
using StripePayment.Application.Queries.Orders;

namespace StripePayment.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("checkout/one-time")]
    public async Task<IActionResult> CreateOneTimeCheckoutAsync(
        [FromBody] CreateCheckoutSessionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCheckoutSessionCommand(
            request.CustomerId,
            request.ProductId,
            false,
            request.SuccessUrl,
            request.CancelUrl);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { Error = result.Error.Code, Message = result.Error.Message });
    }

    [HttpPost("checkout/subscription")]
    public async Task<IActionResult> CreateSubscriptionCheckoutAsync(
        [FromBody] CreateCheckoutSessionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCheckoutSessionCommand(
            request.CustomerId,
            request.ProductId,
            true,
            request.SuccessUrl,
            request.CancelUrl);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { Error = result.Error.Code, Message = result.Error.Message });
    }

    [HttpPost("refund")]
    public async Task<IActionResult> IssueRefundAsync(
        [FromBody] IssueRefundRequest request,
        CancellationToken cancellationToken)
    {
        var command = new IssueRefundCommand(
            request.ChargeId,
            request.Amount,
            request.Reason);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { Error = result.Error.Code, Message = result.Error.Message });
    }

    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderByIdAsync(
        string orderId,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(orderId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { Error = result.Error.Code, Message = result.Error.Message });
    }
}

public record CreateCheckoutSessionRequest(
    string CustomerId,
    string ProductId,
    string? SuccessUrl = null,
    string? CancelUrl = null
);

public record IssueRefundRequest(
    string ChargeId,
    decimal? Amount = null,
    string? Reason = null
);
