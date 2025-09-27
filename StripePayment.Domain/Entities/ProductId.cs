using StripePayment.Domain.Common;

namespace StripePayment.Domain.Entities;

public record ProductId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public static ProductId New() => new(Guid.NewGuid());
    public static ProductId FromString(string value) => new(Guid.Parse(value));
}