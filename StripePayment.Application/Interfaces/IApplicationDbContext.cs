using Microsoft.EntityFrameworkCore;
using StripePayment.Domain.Entities;

namespace StripePayment.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    DbSet<Subscription> Subscriptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}