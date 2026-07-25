namespace OneLine.Billing.Domain.Enums;

public enum SubscriptionStatus
{
    Active = 0,
    Cancelled = 1,
    PastDue = 2,
    Trialing = 3,
    Unpaid = 4,
    Incomplete = 5
}

public enum BillingInterval
{
    Monthly = 0,
    Yearly = 1
}

public enum InvoiceStatus
{
    Draft = 0,
    Open = 1,
    Paid = 2,
    Void = 3,
    Uncollectible = 4
}
