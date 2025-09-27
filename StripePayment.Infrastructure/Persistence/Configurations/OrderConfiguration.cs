using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StripePayment.Domain.Entities;
using StripePayment.Domain.Enums;

namespace StripePayment.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value,
                value => new OrderId(value));

        builder.Property(o => o.CustomerId)
            .HasConversion(
                id => id.Value,
                value => new CustomerId(value));

        builder.Property(o => o.ProductId)
            .HasConversion(
                id => id.Value,
                value => new ProductId(value));

        builder.Property(o => o.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.StripeCheckoutSessionId)
            .HasMaxLength(200);

        builder.Property(o => o.StripePaymentIntentId)
            .HasMaxLength(200);

        builder.Property(o => o.StripeChargeId)
            .HasMaxLength(200);

        builder.HasIndex(o => o.StripeCheckoutSessionId)
            .IsUnique()
            .HasFilter("[StripeCheckoutSessionId] IS NOT NULL");

        builder.HasIndex(o => o.StripeChargeId)
            .IsUnique()
            .HasFilter("[StripeChargeId] IS NOT NULL");
    }
}