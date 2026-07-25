using Microsoft.AspNetCore.Http;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Resolvers;

public sealed class SubdomainTenantResolver : ITenantResolver
{
    private readonly ITenantRepository _repository;
    public int Priority => 3;

    public SubdomainTenantResolver(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid?> ResolveAsync(HttpContext context)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');
        if (parts.Length < 3) return null;

        var subdomain = parts[0].ToLowerInvariant();
        if (subdomain == "www" || subdomain == "localhost") return null;

        var tenant = await _repository.GetBySubdomainAsync(subdomain);
        return tenant?.Id;
    }
}
