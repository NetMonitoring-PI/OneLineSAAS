using Microsoft.AspNetCore.Http;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Resolvers;

/// <summary>
/// Résout le tenant depuis le header HTTP X-Tenant-Id.
/// Exemple : X-Tenant-Id: 3fa85f64-5717-4562-b3fc-2c963f66afa6
/// Pattern : Strategy
/// </summary>
public sealed class HeaderTenantResolver : ITenantResolver
{
    private const string HeaderName = "X-Tenant-Id";
    public int Priority => 1; // Priorité haute

    public Task<Guid?> ResolveAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(
            HeaderName, out var headerValue))
            return Task.FromResult<Guid?>(null);

        if (Guid.TryParse(headerValue, out var tenantId))
            return Task.FromResult<Guid?>(tenantId);

        return Task.FromResult<Guid?>(null);
    }
}
