using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Domain.Errors;

public static class AIErrors
{
    public static readonly Error QuotaExceeded =
        Error.Forbidden(
            "AI.QuotaExceeded",
            "Quota de tokens IA depasse pour ce mois. Mettez a niveau votre plan.");

    public static readonly Error ProviderError =
        Error.Failure(
            "AI.ProviderError",
            "Erreur lors de la communication avec le provider IA.");

    public static readonly Error ConversationNotFound =
        Error.NotFound(
            "AI.ConversationNotFound",
            "Conversation introuvable.");

    public static readonly Error InvalidMessage =
        Error.Validation(
            "AI.InvalidMessage",
            "Le message ne peut pas etre vide.");

    public static readonly Error AINotConfigured =
        Error.Failure(
            "AI.NotConfigured",
            "Le module IA n est pas configure. Verifiez votre cle API.");
}
