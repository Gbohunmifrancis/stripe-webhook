using StripePayment.Domain.Common;

namespace StripePayment.Domain.Entities;

public class Customer : BaseEntity<CustomerId>
{
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? StripeCustomerId { get; set; }
    
    private readonly List<Order> _orders = [];
    private readonly List<Subscription> _subscriptions = [];
    
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

    public void AddOrder(Order order)
    {
        _orders.Add(order);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSubscription(Subscription subscription)
    {
        _subscriptions.Add(subscription);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStripeCustomerId(string stripeCustomerId)
    {
        StripeCustomerId = stripeCustomerId;
        UpdatedAt = DateTime.UtcNow;
    }
}