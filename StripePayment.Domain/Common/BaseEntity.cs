namespace StripePayment.Domain.Common;

public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; init; } = default!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}