using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace OneLine.Observability.Infrastructure.Middleware;

/// <summary>
/// Middleware qui logue chaque requete HTTP avec sa duree.
/// Pattern : Decorator sur le pipeline HTTP
///
/// Logue :
///   - Methode + Path + StatusCode + Duree en ms
///   - Niveau Warning si > 1000ms (requete lente)
///   - Niveau Error si statut >= 500
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;
            var method = context.Request.Method;
            var path = context.Request.Path;

            if (statusCode >= 500)
                _logger.LogError(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
                    method, path, statusCode, elapsed);
            else if (elapsed > 1000)
                _logger.LogWarning(
                    "Slow HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
                    method, path, statusCode, elapsed);
            else
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
                    method, path, statusCode, elapsed);
        }
    }
}
