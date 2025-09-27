using StripePayment.Domain.Common;

namespace StripePayment.Domain.Entities;

public class Product : BaseEntity<ProductId>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "usd";
    public string? StripeProductId { get; set; }
    public string? StripePriceId { get; set; }
    public bool IsRecurring { get; init; }
    public string? RecurringInterval { get; init; } // "month" or "year"
    public bool IsActive { get; set; } = true;

    public void SetStripeIds(string stripeProductId, string stripePriceId)
    {
        StripeProductId = stripeProductId;
        StripePriceId = stripePriceId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}