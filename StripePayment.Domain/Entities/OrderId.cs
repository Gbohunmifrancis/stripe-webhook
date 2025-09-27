using StripePayment.Domain.Common;

namespace StripePayment.Domain.Entities;

public record OrderId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public static OrderId New() => new(Guid.NewGuid());
    public static OrderId FromString(string value) => new(Guid.Parse(value));
}