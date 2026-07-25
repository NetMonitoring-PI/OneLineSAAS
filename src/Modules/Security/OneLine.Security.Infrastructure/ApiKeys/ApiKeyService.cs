using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace OneLine.Security.Infrastructure.ApiKeys;

/// <summary>
/// Service de gestion des API Keys.
/// Pattern : Service
///
/// Les API Keys permettent l acces programmatique sans JWT.
/// Utilise : X-Api-Key header dans les requetes.
/// Hash SHA256 des keys en cache pour validation rapide.
/// </summary>
public interface IApiKeyService
{
    string GenerateApiKey();
    string HashApiKey(string apiKey);
    void RegisterApiKey(string hashedKey, Guid tenantId, string name);
    (bool IsValid, Guid TenantId) ValidateApiKey(string apiKey);
    void RevokeApiKey(string hashedKey);
}

public sealed class ApiKeyService : IApiKeyService
{
    private readonly IMemoryCache _cache;

    public ApiKeyService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GenerateApiKey()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return $"ol_{Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=')}";
    }

    public string HashApiKey(string apiKey)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(apiKey);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public void RegisterApiKey(string hashedKey, Guid tenantId, string name)
    {
        _cache.Set($"apikey_{hashedKey}", (tenantId, name),
            TimeSpan.FromHours(24));
    }

    public (bool IsValid, Guid TenantId) ValidateApiKey(string apiKey)
    {
        var hashed = HashApiKey(apiKey);
        if (_cache.TryGetValue($"apikey_{hashed}", out (Guid TenantId, string Name) entry))
            return (true, entry.TenantId);
        return (false, Guid.Empty);
    }

    public void RevokeApiKey(string hashedKey)
    {
        _cache.Remove($"apikey_{hashedKey}");
    }
}
