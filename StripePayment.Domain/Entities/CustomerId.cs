using StripePayment.Domain.Common;

namespace StripePayment.Domain.Entities;

public record CustomerId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
    public static CustomerId FromString(string value) => new(Guid.Parse(value));
}