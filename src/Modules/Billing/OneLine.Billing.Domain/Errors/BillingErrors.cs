using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Domain.Errors;

public static class BillingErrors
{
    public static readonly Error SubscriptionNotFound =
        Error.NotFound("Billing.SubscriptionNotFound", "Aucun abonnement trouvÃ©.");

    public static readonly Error PlanNotFound =
        Error.NotFound("Billing.PlanNotFound", "Le plan n existe pas.");

    public static readonly Error SubscriptionExpired =
        Error.Forbidden("Billing.SubscriptionExpired", "Abonnement expirÃ©. Veuillez renouveler.");

    public static readonly Error AlreadySubscribed =
        Error.Conflict("Billing.AlreadySubscribed", "Un abonnement actif existe dÃ©jÃ .");

    public static readonly Error InvalidWebhookSignature =
        Error.Unauthorized("Billing.InvalidWebhook", "Signature webhook Stripe invalide.");

    public static readonly Error StripeError =
        Error.Failure("Billing.StripeError", "Erreur lors de la communication avec Stripe.");
}
