using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OneLine.Security.Infrastructure.Options;

namespace OneLine.Security.Infrastructure.BruteForce;

/// <summary>
/// Service de protection contre les attaques brute force.
/// Pattern : Service + IMemoryCache
///
/// Fonctionnement :
///   1. Chaque tentative de login echouee incremente le compteur
///   2. Apres MaxFailedLoginAttempts -> compte bloque
///   3. Lockout expire apres LockoutDurationMinutes
/// </summary>
public interface IBruteForceProtectionService
{
    bool IsLocked(string email);
    void RecordFailedAttempt(string email);
    void RecordSuccessfulLogin(string email);
    int GetFailedAttempts(string email);
}

public sealed class BruteForceProtectionService : IBruteForceProtectionService
{
    private readonly IMemoryCache _cache;
    private readonly SecurityOptions _options;

    public BruteForceProtectionService(
        IMemoryCache cache,
        IOptions<SecurityOptions> options)
    {
        _cache = cache;
        _options = options.Value;
    }

    public bool IsLocked(string email)
    {
        var lockKey = $"lockout_{email.ToLowerInvariant()}";
        return _cache.TryGetValue(lockKey, out _);
    }

    public void RecordFailedAttempt(string email)
    {
        var key = email.ToLowerInvariant();
        var attemptsKey = $"attempts_{key}";
        var lockKey = $"lockout_{key}";

        var attempts = _cache.GetOrCreate(attemptsKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow =
                TimeSpan.FromMinutes(_options.LockoutDurationMinutes);
            return 0;
        });

        attempts++;
        _cache.Set(attemptsKey, attempts,
            TimeSpan.FromMinutes(_options.LockoutDurationMinutes));

        if (attempts >= _options.MaxFailedLoginAttempts)
        {
            _cache.Set(lockKey, true,
                TimeSpan.FromMinutes(_options.LockoutDurationMinutes));
        }
    }

    public void RecordSuccessfulLogin(string email)
    {
        var key = email.ToLowerInvariant();
        _cache.Remove($"attempts_{key}");
        _cache.Remove($"lockout_{key}");
    }

    public int GetFailedAttempts(string email)
    {
        var key = $"attempts_{email.ToLowerInvariant()}";
        return _cache.TryGetValue(key, out int attempts) ? attempts : 0;
    }
}
