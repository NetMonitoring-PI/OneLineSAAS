using Microsoft.AspNetCore.Http;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Middleware;

/// <summary>
/// Middleware qui résout le tenant courant pour chaque requête.
///
/// Pattern : Chain of Responsibility (pipeline middleware)
/// Pattern : Strategy (via ITenantResolver — plusieurs resolvers)
///
/// Ordre d'essai :
///   1. Header X-Tenant-Id (priorité 1)
///   2. JWT Claim tenant_id (priorité 2)
///   3. Subdomain (priorité 3)
///
/// Si résolu → stocké dans HttpContext.Items["TenantId"]
/// Les services injectent ICurrentTenant qui lit depuis Items
/// </summary>
public sealed class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IEnumerable<ITenantResolver> resolvers)
    {
        // Trier par priorité et essayer chaque resolver
        var orderedResolvers = resolvers
            .OrderBy(r => r.Priority);

        foreach (var resolver in orderedResolvers)
        {
            var tenantId = await resolver.ResolveAsync(context);

            if (tenantId.HasValue)
            {
                // Stocker dans HttpContext.Items
                // → accessible partout dans la requête
                context.Items["TenantId"] = tenantId.Value;
                break;
            }
        }

        await _next(context);
    }
}
