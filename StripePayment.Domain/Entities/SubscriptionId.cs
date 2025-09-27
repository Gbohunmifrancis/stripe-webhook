using StripePayment.Domain.Common;

namespace StripePayment.Domain.Entities;

public record SubscriptionId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public static SubscriptionId New() => new(Guid.NewGuid());
    public static SubscriptionId FromString(string value) => new(Guid.Parse(value));
}