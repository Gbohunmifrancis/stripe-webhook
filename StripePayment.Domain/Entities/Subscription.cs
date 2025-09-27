using StripePayment.Domain.Common;
using StripePayment.Domain.Enums;

namespace StripePayment.Domain.Entities;

public class Subscription : BaseEntity<SubscriptionId>
{
    public CustomerId CustomerId { get; init; } = default!;
    public ProductId ProductId { get; init; } = default!;
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Pending;
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCheckoutSessionId { get; set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? NextBillingDate { get; set; }
    public DateTime? CancelledAt { get; private set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = default!;
    public Product Product { get; set; } = default!;

    public void SetStripeCheckoutSessionId(string sessionId)
    {
        StripeCheckoutSessionId = sessionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStripeSubscriptionId(string subscriptionId)
    {
        StripeSubscriptionId = subscriptionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate(DateTime startDate, DateTime? nextBillingDate = null)
    {
        Status = SubscriptionStatus.Active;
        StartDate = startDate;
        NextBillingDate = nextBillingDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsExpired()
    {
        Status = SubscriptionStatus.Expired;
        EndDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPastDue()
    {
        Status = SubscriptionStatus.PastDue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnpaid()
    {
        Status = SubscriptionStatus.Unpaid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNextBillingDate(DateTime nextBillingDate)
    {
        NextBillingDate = nextBillingDate;
        UpdatedAt = DateTime.UtcNow;
    }
}