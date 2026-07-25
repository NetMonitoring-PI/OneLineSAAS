using Microsoft.AspNetCore.Http;
using OneLine.Security.Infrastructure.ApiKeys;

namespace OneLine.Security.Infrastructure.Middleware;

/// <summary>
/// Middleware qui valide le header X-Api-Key.
/// Si la key est valide, injecte le TenantId dans HttpContext.Items.
/// Complementaire au JWT - l un ou l autre suffit.
/// </summary>
public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeader = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IApiKeyService apiKeyService)
    {
        if (context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey))
        {
            var (isValid, tenantId) = apiKeyService.ValidateApiKey(apiKey!);
            if (isValid)
            {
                context.Items["TenantId"] = tenantId;
                context.Items["ApiKeyAuthenticated"] = true;
            }
        }

        await _next(context);
    }
}
