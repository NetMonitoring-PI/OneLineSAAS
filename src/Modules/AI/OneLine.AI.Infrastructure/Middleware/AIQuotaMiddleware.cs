using Microsoft.AspNetCore.Http;
using OneLine.AI.Domain.Interfaces;
using OneLine.Shared.Domain.Interfaces;

namespace OneLine.AI.Infrastructure.Middleware;

/// <summary>
/// Middleware qui verifie le quota de tokens IA avant chaque requete AI.
/// Retourne HTTP 429 si le quota mensuel est depasse.
///
/// Ne s applique qu aux routes /api/ai/*
/// Les autres routes ne sont pas affectees.
/// </summary>
public sealed class AIQuotaMiddleware
{
    private readonly RequestDelegate _next;
    private const int DefaultMonthlyQuota = 50_000;

    public AIQuotaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenant currentTenant,
        IAIUsageRepository usageRepo)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Appliquer uniquement aux routes AI
        if (!path.StartsWith("/api/ai", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!currentTenant.IsResolved)
        {
            await _next(context);
            return;
        }

        var monthlyTokens = await usageRepo
            .GetMonthlyTokensAsync(currentTenant.TenantId);

        if (monthlyTokens >= DefaultMonthlyQuota)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                $"{{\"code\":\"AI.QuotaExceeded\"," +
                $"\"message\":\"Quota mensuel de {DefaultMonthlyQuota} tokens depasse.\"," +
                $"\"used\":{monthlyTokens},\"quota\":{DefaultMonthlyQuota}}}");
            return;
        }

        await _next(context);
    }
}
