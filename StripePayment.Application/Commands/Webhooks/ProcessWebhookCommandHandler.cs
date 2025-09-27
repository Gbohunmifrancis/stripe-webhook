using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StripePayment.Application.Common;
using StripePayment.Application.Interfaces;
using StripePayment.Domain.Enums;

namespace StripePayment.Application.Commands.Webhooks;

public sealed class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, Result>
{
    private readonly IPaymentService _paymentService;
    private readonly IApplicationDbContext _context;

    public ProcessWebhookCommandHandler(
        IPaymentService paymentService,
        IApplicationDbContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    public async Task<Result> Handle(
        ProcessWebhookCommand request,
        CancellationToken cancellationToken)
    {
        // Parse the webhook payload
        JsonDocument eventData;
        try
        {
            eventData = JsonDocument.Parse(request.Payload);
        }
        catch (JsonException)
        {
            return Result.Failure(
                new Error("Webhook.InvalidPayload", "Invalid JSON payload"));
        }

        var eventType = request.EventType;
        var dataObject = eventData.RootElement.GetProperty("data").GetProperty("object");

        return eventType switch
        {
            "checkout.session.completed" => await HandleCheckoutSessionCompletedAsync(dataObject, cancellationToken),
            "invoice.payment_succeeded" => await HandleInvoicePaymentSucceededAsync(dataObject, cancellationToken),
            _ => Result.Success() // Ignore unknown events
        };
    }

    private async Task<Result> HandleCheckoutSessionCompletedAsync(
        JsonElement sessionObject,
        CancellationToken cancellationToken)
    {
        var sessionId = sessionObject.GetProperty("id").GetString();
        var paymentIntentId = sessionObject.TryGetProperty("payment_intent", out var piElement) 
            ? piElement.GetString() 
            : null;

        if (string.IsNullOrEmpty(sessionId))
        {
            return Result.Failure(
                new Error("Webhook.MissingSessionId", "Session ID is missing from webhook data"));
        }

        // Handle subscription checkout completion
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeCheckoutSessionId == sessionId, cancellationToken);

        if (subscription is not null)
        {
            var stripeSubscriptionId = sessionObject.TryGetProperty("subscription", out var subElement) 
                ? subElement.GetString() 
                : null;

            if (!string.IsNullOrEmpty(stripeSubscriptionId))
            {
                subscription.SetStripeSubscriptionId(stripeSubscriptionId);
                subscription.Activate(DateTime.UtcNow);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        // Handle one-time payment checkout completion
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.StripeCheckoutSessionId == sessionId, cancellationToken);

        if (order is not null && !string.IsNullOrEmpty(paymentIntentId))
        {
            // In a real scenario, you'd also get the charge ID from Stripe
            var chargeId = $"ch_{Guid.NewGuid():N}"; // Placeholder
            order.SetPaymentCompleted(paymentIntentId, chargeId);
            order.MarkAsFulfilled();

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        return Result.Failure(
            new Error("Webhook.OrderNotFound", "Order or subscription not found for checkout session"));
    }

    private async Task<Result> HandleInvoicePaymentSucceededAsync(
        JsonElement invoiceObject,
        CancellationToken cancellationToken)
    {
        var subscriptionId = invoiceObject.TryGetProperty("subscription", out var subElement) 
            ? subElement.GetString() 
            : null;

        if (string.IsNullOrEmpty(subscriptionId))
        {
            return Result.Failure(
                new Error("Webhook.MissingSubscriptionId", "Subscription ID is missing from webhook data"));
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscriptionId, cancellationToken);

        if (subscription is null)
        {
            return Result.Failure(
                new Error("Webhook.SubscriptionNotFound", "Subscription not found"));
        }

        // Update subscription billing information
        if (invoiceObject.TryGetProperty("lines", out var linesElement) &&
            linesElement.GetProperty("data").GetArrayLength() > 0)
        {
            var firstLine = linesElement.GetProperty("data")[0];
            if (firstLine.TryGetProperty("period", out var periodElement) &&
                periodElement.TryGetProperty("end", out var endElement))
            {
                var nextBillingTimestamp = endElement.GetInt64();
                var nextBillingDate = DateTimeOffset.FromUnixTimeSeconds(nextBillingTimestamp).DateTime;
                subscription.UpdateNextBillingDate(nextBillingDate);
            }
        }

        // Ensure subscription is active
        if (subscription.Status != SubscriptionStatus.Active)
        {
            subscription.Activate(DateTime.UtcNow);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}