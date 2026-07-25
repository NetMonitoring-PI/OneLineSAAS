using Microsoft.Extensions.Options;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Infrastructure.Options;
using Stripe;
using Stripe.Checkout;

namespace OneLine.Billing.Infrastructure.Services;

/// <summary>
/// ImplÃ©mentation concrÃ¨te de IStripeService.
/// Tous les appels Stripe sont ici -- Application n a pas besoin de savoir.
/// </summary>
public sealed class StripeService : IStripeService
{
    private readonly StripeOptions _options;

    public StripeService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<string> CreateCustomerAsync(
        string email, string tenantName, CancellationToken ct = default)
    {
        var service = new CustomerService();
        var customer = await service.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Name = tenantName,
            Metadata = new Dictionary<string, string> { ["tenant_name"] = tenantName }
        }, cancellationToken: ct);
        return customer.Id;
    }

    public async Task<(string SubscriptionId, DateTime PeriodEnd)> CreateSubscriptionAsync(
        string customerId, string stripePriceId, CancellationToken ct = default)
    {
        var service = new SubscriptionService();
        var subscription = await service.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = [new SubscriptionItemOptions { Price = stripePriceId }],
            PaymentBehavior = "default_incomplete",
            Expand = ["latest_invoice.payment_intent"]
        }, cancellationToken: ct);

        return (subscription.Id,
                subscription.CurrentPeriodEnd);
    }

    public async Task CancelSubscriptionAsync(
        string stripeSubscriptionId, CancellationToken ct = default)
    {
        var service = new SubscriptionService();
        await service.CancelAsync(stripeSubscriptionId, cancellationToken: ct);
    }

    public async Task<string> CreateCheckoutSessionAsync(
        string customerId, string stripePriceId,
        string successUrl, string cancelUrl,
        CancellationToken ct = default)
    {
        var service = new SessionService();
        var session = await service.CreateAsync(new SessionCreateOptions
        {
            Customer = customerId,
            Mode = "subscription",
            LineItems = [new SessionLineItemOptions { Price = stripePriceId, Quantity = 1 }],
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
        }, cancellationToken: ct);
        return session.Url;
    }

    public bool ValidateWebhookSignature(
        string payload, string signature, string secret,
        out string eventType, out string eventJson)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, secret);
            eventType = stripeEvent.Type;
            eventJson = stripeEvent.ToJson();
            return true;
        }
        catch
        {
            eventType = string.Empty;
            eventJson = string.Empty;
            return false;
        }
    }
}
