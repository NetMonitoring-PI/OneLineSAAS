using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OneLine.Security.Infrastructure.Options;

namespace OneLine.Security.Infrastructure.RateLimiting;

/// <summary>
/// Middleware de rate limiting par IP.
/// Pattern : Chain of Responsibility (pipeline middleware)
/// Utilise IMemoryCache pour tracker les compteurs par IP.
/// Retourne HTTP 429 Too Many Requests si limite depassee.
/// </summary>
public sealed class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly SecurityOptions _options;

    public RateLimitMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        IOptions<SecurityOptions> options)
    {
        _next = next;
        _cache = cache;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rate_limit_{ip}";
        var window = TimeSpan.FromMinutes(1);

        var count = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = window;
            return 0;
        });

        count++;
        _cache.Set(key, count, window);

        if (count > _options.MaxRequestsPerMinutePerIp)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = "60";
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"code\":\"Security.RateLimitExceeded\"," +
                "\"message\":\"Trop de requetes. Reessayez dans 60 secondes.\"}");
            return;
        }

        await _next(context);
    }
}
