using OneLine.Billing.Domain.Enums;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.Billing.Domain.Entities;

/// <summary>
/// Facture gÃ©nÃ©rÃ©e par Stripe.
/// CrÃ©Ã©e automatiquement via les webhooks Stripe.
/// </summary>
public sealed class Invoice : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "usd";
    public InvoiceStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? StripeInvoiceId { get; private set; }
    public string? StripeHostedUrl { get; private set; }

    private Invoice() { }

    public static Invoice Create(
        Guid tenantId,
        Guid subscriptionId,
        decimal amount,
        string stripeInvoiceId,
        string? hostedUrl = null,
        string currency = "usd")
    {
        return new Invoice
        {
            TenantId = tenantId,
            SubscriptionId = subscriptionId,
            Amount = amount,
            Currency = currency,
            Status = InvoiceStatus.Open,
            StripeInvoiceId = stripeInvoiceId,
            StripeHostedUrl = hostedUrl
        };
    }

    public void MarkAsPaid()
    {
        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkAsVoid()
    {
        Status = InvoiceStatus.Void;
        SetUpdatedAt();
    }
}
