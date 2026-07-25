using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace OneLine.Observability.Infrastructure.Middleware;

/// <summary>
/// Middleware qui genere un X-Correlation-Id unique pour chaque requete.
///
/// Role :
///   - Permet de tracer une requete a travers tous les logs
///   - Retourne le CorrelationId dans la reponse HTTP
///   - Enrichit le contexte Serilog pour tous les logs de la requete
///
/// Usage client :
///   - Lire X-Correlation-Id dans la reponse
///   - Renvoyer dans les requetes suivantes pour le debugging
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Recuperer depuis le header ou generer un nouveau
        var correlationId = context.Request.Headers
            .TryGetValue(CorrelationIdHeader, out var existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString();

        // Ajouter a la reponse pour que le client puisse tracer
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Enrichir le contexte Serilog
        // Tous les logs dans cette requete auront CorrelationId
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        {
            await _next(context);
        }
    }
}
