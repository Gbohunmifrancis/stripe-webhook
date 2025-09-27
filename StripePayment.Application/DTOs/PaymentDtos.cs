namespace StripePayment.Application.DTOs;

public record CheckoutSessionResponse(
    string SessionId,
    string SessionUrl
);

public record RefundResponse(
    string RefundId,
    decimal Amount,
    string Status
);

public record PaymentMethodRequest(
    string CustomerId,
    string ProductId,
    string? SuccessUrl = null,
    string? CancelUrl = null
);

public record RefundRequest(
    string ChargeId,
    decimal? Amount = null,
    string? Reason = null
);

public record WebhookEventRequest(
    string EventType,
    Dictionary<string, object> Data
);