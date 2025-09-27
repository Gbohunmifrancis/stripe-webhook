namespace StripePayment.Domain.Enums;

public enum SubscriptionStatus
{
    Pending = 0,
    Active = 1,
    Cancelled = 2,
    Expired = 3,
    PastDue = 4,
    Unpaid = 5
}