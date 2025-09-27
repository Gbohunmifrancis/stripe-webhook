using StripePayment.Domain.Common;
using StripePayment.Domain.Enums;

namespace StripePayment.Domain.Entities;

public class Order : BaseEntity<OrderId>
{
    public CustomerId CustomerId { get; init; } = default!;
    public ProductId ProductId { get; init; } = default!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "usd";
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripeChargeId { get; set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? FulfilledAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = default!;
    public Product Product { get; set; } = default!;

    public void SetStripeCheckoutSessionId(string sessionId)
    {
        StripeCheckoutSessionId = sessionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPaymentCompleted(string paymentIntentId, string chargeId)
    {
        Status = OrderStatus.PaymentCompleted;
        StripePaymentIntentId = paymentIntentId;
        StripeChargeId = chargeId;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFulfilled()
    {
        if (Status != OrderStatus.PaymentCompleted)
        {
            throw new InvalidOperationException("Order must be paid before it can be fulfilled.");
        }

        Status = OrderStatus.Fulfilled;
        FulfilledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsRefunded()
    {
        if (Status != OrderStatus.PaymentCompleted && Status != OrderStatus.Fulfilled)
        {
            throw new InvalidOperationException("Only completed or fulfilled orders can be refunded.");
        }

        Status = OrderStatus.Refunded;
        RefundedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}