using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StripePayment.Application.Common;
using StripePayment.Application.DTOs;
using StripePayment.Application.Interfaces;
using StripePayment.Infrastructure.Configuration;

namespace StripePayment.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly StripeOptions _stripeOptions;

    public StripePaymentService(IOptions<StripeOptions> stripeOptions)
    {
        _stripeOptions = stripeOptions.Value;
        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;
    }

    public async Task<Result<CheckoutSessionResponse>> CreateOneTimePaymentSessionAsync(
        PaymentMethodRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems = 
                [
                    new SessionLineItemOptions
                    {
                        Price = request.ProductId, // Assuming ProductId is actually the Stripe Price ID
                        Quantity = 1,
                    }
                ],
                Mode = "payment",
                SuccessUrl = request.SuccessUrl ?? _stripeOptions.SuccessUrl,
                CancelUrl = request.CancelUrl ?? _stripeOptions.CancelUrl,
                Customer = request.CustomerId, // Assuming CustomerId is actually the Stripe Customer ID
                Metadata = new Dictionary<string, string>
                {
                    ["customer_id"] = request.CustomerId,
                    ["product_id"] = request.ProductId
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, null, cancellationToken);

            return Result.Success(new CheckoutSessionResponse(session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Stripe.PaymentSessionFailed", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Payment.UnexpectedError", ex.Message));
        }
    }

    public async Task<Result<CheckoutSessionResponse>> CreateSubscriptionSessionAsync(
        PaymentMethodRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems = 
                [
                    new SessionLineItemOptions
                    {
                        Price = request.ProductId, // Assuming ProductId is actually the Stripe Price ID
                        Quantity = 1,
                    }
                ],
                Mode = "subscription",
                SuccessUrl = request.SuccessUrl ?? _stripeOptions.SuccessUrl,
                CancelUrl = request.CancelUrl ?? _stripeOptions.CancelUrl,
                Customer = request.CustomerId, // Assuming CustomerId is actually the Stripe Customer ID
                Metadata = new Dictionary<string, string>
                {
                    ["customer_id"] = request.CustomerId,
                    ["product_id"] = request.ProductId
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, null, cancellationToken);

            return Result.Success(new CheckoutSessionResponse(session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Stripe.SubscriptionSessionFailed", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<CheckoutSessionResponse>(
                new Error("Payment.UnexpectedError", ex.Message));
        }
    }

    public async Task<Result<RefundResponse>> IssueRefundAsync(
        RefundRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                Charge = request.ChargeId,
                Amount = request.Amount.HasValue ? (long)(request.Amount.Value * 100) : null,
                Reason = request.Reason switch
                {
                    "duplicate" => "duplicate",
                    "fraudulent" => "fraudulent",
                    _ => "requested_by_customer"
                }
            };

            var service = new RefundService();
            var refund = await service.CreateAsync(options, null, cancellationToken);

            return Result.Success(new RefundResponse(
                refund.Id,
                refund.Amount / 100m,
                refund.Status));
        }
        catch (StripeException ex)
        {
            return Result.Failure<RefundResponse>(
                new Error("Stripe.RefundFailed", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<RefundResponse>(
                new Error("Payment.UnexpectedError", ex.Message));
        }
    }

    public async Task<Result> ProcessWebhookAsync(
        WebhookEventRequest webhookEvent,
        string signature,
        CancellationToken cancellationToken = default)
    {
        // This method is mainly handled by the webhook command handler
        // The actual webhook processing logic is in ProcessWebhookCommandHandler
        await Task.CompletedTask;
        return Result.Success();
    }

    public bool VerifyWebhookSignature(string payload, string signature, string secret)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, secret);
            return true;
        }
        catch (StripeException)
        {
            return false;
        }
    }
}