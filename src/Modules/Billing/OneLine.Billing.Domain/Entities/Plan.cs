using OneLine.Billing.Domain.Enums;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.Billing.Domain.Entities;

/// <summary>
/// ReprÃ©sente un plan tarifaire SaaS.
/// Ex: Free, Starter (9$/mois), Pro (29$/mois), Enterprise
/// </summary>
public sealed class Plan : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public BillingInterval Interval { get; private set; }
    public int TokenQuota { get; private set; }
    public bool IsActive { get; private set; }
    public string? StripeProductId { get; private set; }
    public string? StripePriceId { get; private set; }

    private Plan() { }

    public static Plan Create(
        string name,
        string description,
        decimal price,
        BillingInterval interval,
        int tokenQuota = 10000,
        string? stripeProductId = null,
        string? stripePriceId = null)
    {
        return new Plan
        {
            Name = name,
            Description = description,
            Price = price,
            Interval = interval,
            TokenQuota = tokenQuota,
            IsActive = true,
            StripeProductId = stripeProductId,
            StripePriceId = stripePriceId
        };
    }

    public void Deactivate() { IsActive = false; SetUpdatedAt(); }
    public void SetStripeIds(string productId, string priceId)
    {
        StripeProductId = productId;
        StripePriceId = priceId;
        SetUpdatedAt();
    }
}
