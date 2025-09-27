using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StripePayment.Domain.Entities;

namespace StripePayment.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new CustomerId(value));

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.StripeCustomerId)
            .HasMaxLength(100);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.StripeCustomerId)
            .IsUnique()
            .HasFilter("[StripeCustomerId] IS NOT NULL");

        // Configure navigation properties
        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Subscriptions)
            .WithOne(s => s.Customer)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore private fields
        builder.Ignore("_orders");
        builder.Ignore("_subscriptions");
    }
}