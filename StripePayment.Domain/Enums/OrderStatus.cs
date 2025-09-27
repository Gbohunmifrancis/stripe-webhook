namespace StripePayment.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    PaymentCompleted = 1,
    Fulfilled = 2,
    Cancelled = 3,
    Refunded = 4
}