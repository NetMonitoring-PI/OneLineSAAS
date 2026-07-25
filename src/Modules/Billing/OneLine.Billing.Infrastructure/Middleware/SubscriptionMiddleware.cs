using Microsoft.AspNetCore.Http;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Middleware;

/// <summary>
/// Middleware qui vÃ©rifie que le tenant a un abonnement actif.
/// Retourne HTTP 402 Payment Required si abonnement expirÃ©.
///
/// IgnorÃ© pour les routes publiques : /auth, /billing/webhook, /swagger
/// </summary>
public sealed class SubscriptionMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly string[] _ignoredPaths =
    [
        "/api/auth",
        "/api/billing/webhook",
        "/swagger",
        "/health"
    ];

    public SubscriptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenant currentTenant,
        ISubscriptionRepository subscriptionRepo)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Ignorer les routes publiques
        if (_ignoredPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        // Si pas de tenant rÃ©solu â†’ laisser passer (Auth s en occupe)
        if (!currentTenant.IsResolved)
        {
            await _next(context);
            return;
        }

        // VÃ©rifier abonnement actif
        var hasActive = await subscriptionRepo
            .HasActiveSubscriptionAsync(currentTenant.TenantId);

        if (!hasActive)
        {
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"code\":\"Billing.SubscriptionExpired\"," +
                "\"message\":\"Abonnement expirÃ©. Veuillez renouveler votre abonnement.\"}");
            return;
        }

        await _next(context);
    }
}
