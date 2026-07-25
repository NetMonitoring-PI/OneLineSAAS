using Microsoft.AspNetCore.Http;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Resolvers;

public sealed class HeaderTenantResolver : ITenantResolver
{
    private const string HeaderName = "X-Tenant-Id";
    public int Priority => 1;

    public Task<Guid?> ResolveAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var headerValue))
            return Task.FromResult<Guid?>(null);

        if (Guid.TryParse(headerValue, out var tenantId))
            return Task.FromResult<Guid?>(tenantId);

        return Task.FromResult<Guid?>(null);
    }
}
