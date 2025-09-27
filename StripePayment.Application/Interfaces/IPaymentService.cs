using StripePayment.Application.Common;
using StripePayment.Application.DTOs;

namespace StripePayment.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<CheckoutSessionResponse>> CreateOneTimePaymentSessionAsync(
        PaymentMethodRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<CheckoutSessionResponse>> CreateSubscriptionSessionAsync(
        PaymentMethodRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<RefundResponse>> IssueRefundAsync(
        RefundRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ProcessWebhookAsync(
        WebhookEventRequest webhookEvent,
        string signature,
        CancellationToken cancellationToken = default);

    bool VerifyWebhookSignature(string payload, string signature, string secret);
}