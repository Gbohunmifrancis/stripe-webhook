using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StripePayment.Domain.Entities;

namespace StripePayment.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new ProductId(value));

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.StripeProductId)
            .HasMaxLength(100);

        builder.Property(p => p.StripePriceId)
            .HasMaxLength(100);

        builder.Property(p => p.RecurringInterval)
            .HasMaxLength(20);

        builder.HasIndex(p => p.StripeProductId)
            .IsUnique()
            .HasFilter("[StripeProductId] IS NOT NULL");

        builder.HasIndex(p => p.StripePriceId)
            .IsUnique()
            .HasFilter("[StripePriceId] IS NOT NULL");
    }
}