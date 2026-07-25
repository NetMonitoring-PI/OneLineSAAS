using Microsoft.AspNetCore.Http;

namespace OneLine.Tenants.Domain.Interfaces;

/// <summary>
/// Pattern Strategy — interface commune pour tous les resolvers.
/// Chaque resolver implémente sa propre logique de détection.
/// </summary>
public interface ITenantResolver
{
    /// <summary>Priorité — le resolver avec la plus haute priorité est essayé en premier</summary>
    int Priority { get; }

    /// <summary>Tenter de résoudre le TenantId depuis la requête HTTP</summary>
    Task<Guid?> ResolveAsync(HttpContext context);
}
