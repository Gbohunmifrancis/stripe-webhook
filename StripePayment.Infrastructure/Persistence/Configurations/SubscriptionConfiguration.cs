using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StripePayment.Domain.Entities;
using StripePayment.Domain.Enums;

namespace StripePayment.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SubscriptionId(value));

        builder.Property(s => s.CustomerId)
            .HasConversion(
                id => id.Value,
                value => new CustomerId(value));

        builder.Property(s => s.ProductId)
            .HasConversion(
                id => id.Value,
                value => new ProductId(value));

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.StripeSubscriptionId)
            .HasMaxLength(200);

        builder.Property(s => s.StripeCheckoutSessionId)
            .HasMaxLength(200);

        builder.HasIndex(s => s.StripeSubscriptionId)
            .IsUnique()
            .HasFilter("[StripeSubscriptionId] IS NOT NULL");

        builder.HasIndex(s => s.StripeCheckoutSessionId)
            .IsUnique()
            .HasFilter("[StripeCheckoutSessionId] IS NOT NULL");
    }
}