namespace OneLine.Billing.Application.Interfaces;

/// <summary>
/// Abstraction du service Stripe.
/// Pattern Strategy -- permet de changer de provider sans toucher Application.
/// </summary>
public interface IStripeService
{
    /// <summary>CrÃ©er un client Stripe pour le tenant</summary>
    Task<string> CreateCustomerAsync(string email, string tenantName, CancellationToken ct = default);

    /// <summary>CrÃ©er un abonnement Stripe</summary>
    Task<(string SubscriptionId, DateTime PeriodEnd)> CreateSubscriptionAsync(
        string customerId, string stripePriceId, CancellationToken ct = default);

    /// <summary>Annuler un abonnement Stripe</summary>
    Task CancelSubscriptionAsync(string stripeSubscriptionId, CancellationToken ct = default);

    /// <summary>CrÃ©er une session de paiement Checkout</summary>
    Task<string> CreateCheckoutSessionAsync(
        string customerId, string stripePriceId,
        string successUrl, string cancelUrl,
        CancellationToken ct = default);

    /// <summary>Valider la signature d'un webhook Stripe</summary>
    bool ValidateWebhookSignature(string payload, string signature, string secret, out string eventType, out string eventJson);
}
