using OneLine.Billing.Domain.Enums;
using OneLine.Billing.Domain.Events;
using OneLine.Shared.Domain.Primitives;

namespace OneLine.Billing.Domain.Entities;

/// <summary>
/// Abonnement d un tenant a un plan.
/// LiÃ© a Stripe via StripeSubscriptionId et StripeCustomerId.
///
/// Cycle de vie :
///   1. Tenant s inscrit -> Subscription crÃ©Ã©e (Trialing)
///   2. Stripe prÃ©lÃ¨ve -> Active
///   3. Paiement Ã©chouÃ© -> PastDue
///   4. Tenant annule -> Cancelled
/// </summary>
public sealed class Subscription : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime CurrentPeriodStart { get; private set; }
    public DateTime CurrentPeriodEnd { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? TrialEndsAt { get; private set; }
    public string? StripeSubscriptionId { get; private set; }
    public string? StripeCustomerId { get; private set; }

    public bool IsActive => Status == SubscriptionStatus.Active
                         || Status == SubscriptionStatus.Trialing;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    public void ClearDomainEvents() => _domainEvents.Clear();

    // Navigation
    public Plan? Plan { get; private set; }

    private Subscription() { }

    public static Subscription Create(
        Guid tenantId,
        Guid planId,
        string? stripeCustomerId = null,
        int trialDays = 14)
    {
        var now = DateTime.UtcNow;
        var sub = new Subscription
        {
            TenantId = tenantId,
            PlanId = planId,
            Status = SubscriptionStatus.Trialing,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = now.AddDays(trialDays),
            TrialEndsAt = now.AddDays(trialDays),
            StripeCustomerId = stripeCustomerId
        };

        sub._domainEvents.Add(new SubscriptionCreatedEvent(sub.Id, tenantId, string.Empty));
        return sub;
    }

    // AppelÃ© par webhook Stripe : invoice.payment_succeeded
    public void Activate(DateTime periodEnd, string stripeSubscriptionId)
    {
        Status = SubscriptionStatus.Active;
        CurrentPeriodEnd = periodEnd;
        StripeSubscriptionId = stripeSubscriptionId;
        SetUpdatedAt();
    }

    // AppelÃ© par webhook Stripe : invoice.payment_failed
    public void MarkAsPastDue()
    {
        Status = SubscriptionStatus.PastDue;
        SetUpdatedAt();
    }

    // AppelÃ© par webhook Stripe : customer.subscription.deleted
    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        _domainEvents.Add(new SubscriptionCancelledEvent(Id, TenantId));
        SetUpdatedAt();
    }

    public void SetStripeSubscriptionId(string id)
    {
        StripeSubscriptionId = id;
        SetUpdatedAt();
    }
}
