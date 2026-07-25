using Microsoft.AspNetCore.Http;
using OneLine.Tenants.Domain.Interfaces;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Resolvers;

/// <summary>
/// Résout le tenant depuis le sous-domaine.
/// Exemple : client1.votreapp.com → subdomain = "client1"
/// Pattern : Strategy
/// </summary>
public sealed class SubdomainTenantResolver : ITenantResolver
{
    private readonly ITenantRepository _repository;
    public int Priority => 3; // Priorité basse

    public SubdomainTenantResolver(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid?> ResolveAsync(HttpContext context)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');

        // Besoin d'au moins : subdomain.domain.tld
        if (parts.Length < 3)
            return null;

        var subdomain = parts[0].ToLowerInvariant();

        // Ignorer "www"
        if (subdomain == "www" || subdomain == "localhost")
            return null;

        var tenant = await _repository.GetBySubdomainAsync(subdomain);
        return tenant?.Id;
    }
}
