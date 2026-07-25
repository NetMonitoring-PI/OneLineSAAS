using Microsoft.AspNetCore.Http;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Resolvers;

public sealed class ClaimTenantResolver : ITenantResolver
{
    private const string ClaimName = "tenant_id";
    public int Priority => 2;

    public Task<Guid?> ResolveAsync(HttpContext context)
    {
        var claim = context.User.FindFirst(ClaimName);
        if (claim is null) return Task.FromResult<Guid?>(null);

        if (Guid.TryParse(claim.Value, out var tenantId))
            return Task.FromResult<Guid?>(tenantId);

        return Task.FromResult<Guid?>(null);
    }
}
