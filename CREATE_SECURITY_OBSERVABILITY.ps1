# ============================================================
# Script Module Security + Observability
# Executer depuis : C:\Users\DELL\Projects\OneLine.SaasKit
# ============================================================

Write-Host "=== Module Security + Observability ===" -ForegroundColor Cyan

# ── ETAPE 1 : Creer les projets ──────────────────────────────
Write-Host "`n[1/6] Creation des projets..." -ForegroundColor Yellow

dotnet new classlib -n OneLine.Security.Infrastructure -o src\Modules\Security\OneLine.Security.Infrastructure --force
dotnet new classlib -n OneLine.Observability.Infrastructure -o src\Modules\Observability\OneLine.Observability.Infrastructure --force

dotnet sln add src\Modules\Security\OneLine.Security.Infrastructure\OneLine.Security.Infrastructure.csproj
dotnet sln add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj

Remove-Item -Force src\Modules\Security\OneLine.Security.Infrastructure\Class1.cs -ErrorAction SilentlyContinue
Remove-Item -Force src\Modules\Observability\OneLine.Observability.Infrastructure\Class1.cs -ErrorAction SilentlyContinue

Write-Host "Projets crees" -ForegroundColor Green

# ── ETAPE 2 : References et packages ─────────────────────────
Write-Host "`n[2/6] References et packages..." -ForegroundColor Yellow

# Security
dotnet add src\Modules\Security\OneLine.Security.Infrastructure\OneLine.Security.Infrastructure.csproj reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\Security\OneLine.Security.Infrastructure\OneLine.Security.Infrastructure.csproj package Microsoft.AspNetCore.Http --version 2.2.2
dotnet add src\Modules\Security\OneLine.Security.Infrastructure\OneLine.Security.Infrastructure.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 9.0.0
dotnet add src\Modules\Security\OneLine.Security.Infrastructure\OneLine.Security.Infrastructure.csproj package Microsoft.Extensions.Caching.Memory --version 9.0.0
dotnet add src\Modules\Security\OneLine.Security.Infrastructure\OneLine.Security.Infrastructure.csproj package Microsoft.Extensions.Configuration.Abstractions --version 9.0.0

# Observability
dotnet add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj reference src\Shared\OneLine.Shared.Domain\OneLine.Shared.Domain.csproj
dotnet add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj package Serilog.AspNetCore --version 8.0.3
dotnet add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj package Serilog.Sinks.Console --version 6.0.0
dotnet add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj package Serilog.Sinks.File --version 6.0.0
dotnet add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj package prometheus-net.AspNetCore --version 8.2.1
dotnet add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 9.0.0
dotnet add src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj package Microsoft.AspNetCore.Http --version 2.2.2

# API
dotnet add src\OneLine.API\OneLine.API.csproj reference src\Modules\Security\OneLine.Security.Infrastructure\OneLine.Security.Infrastructure.csproj
dotnet add src\OneLine.API\OneLine.API.csproj reference src\Modules\Observability\OneLine.Observability.Infrastructure\OneLine.Observability.Infrastructure.csproj

Write-Host "References OK" -ForegroundColor Green

# ── ETAPE 3 : Creer les dossiers ─────────────────────────────
Write-Host "`n[3/6] Creation des dossiers..." -ForegroundColor Yellow

$dirs = @(
    "src\Modules\Security\OneLine.Security.Infrastructure\RateLimiting",
    "src\Modules\Security\OneLine.Security.Infrastructure\BruteForce",
    "src\Modules\Security\OneLine.Security.Infrastructure\ApiKeys",
    "src\Modules\Security\OneLine.Security.Infrastructure\Middleware",
    "src\Modules\Security\OneLine.Security.Infrastructure\Options",
    "src\Modules\Observability\OneLine.Observability.Infrastructure\Logging",
    "src\Modules\Observability\OneLine.Observability.Infrastructure\Middleware",
    "src\Modules\Observability\OneLine.Observability.Infrastructure\Metrics"
)
foreach ($dir in $dirs) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

Write-Host "Dossiers crees" -ForegroundColor Green

# ── ETAPE 4 : Creer les fichiers ─────────────────────────────
Write-Host "`n[4/6] Creation des fichiers..." -ForegroundColor Yellow

# ── SECURITY ────────────────────────────────────────────────

Set-Content -Path "src\Modules\Security\OneLine.Security.Infrastructure\Options\SecurityOptions.cs" -Encoding UTF8 -Value @'
namespace OneLine.Security.Infrastructure.Options;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";
    public int MaxRequestsPerMinutePerIp { get; set; } = 60;
    public int MaxRequestsPerMinutePerUser { get; set; } = 100;
    public int MaxFailedLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 15;
}
'@

Set-Content -Path "src\Modules\Security\OneLine.Security.Infrastructure\RateLimiting\RateLimitMiddleware.cs" -Encoding UTF8 -Value @'
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
'@

Set-Content -Path "src\Modules\Security\OneLine.Security.Infrastructure\BruteForce\BruteForceProtectionService.cs" -Encoding UTF8 -Value @'
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
'@

Set-Content -Path "src\Modules\Security\OneLine.Security.Infrastructure\ApiKeys\ApiKeyService.cs" -Encoding UTF8 -Value @'
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
'@

Set-Content -Path "src\Modules\Security\OneLine.Security.Infrastructure\Middleware\ApiKeyMiddleware.cs" -Encoding UTF8 -Value @'
using Microsoft.AspNetCore.Http;
using OneLine.Security.Infrastructure.ApiKeys;

namespace OneLine.Security.Infrastructure.Middleware;

/// <summary>
/// Middleware qui valide le header X-Api-Key.
/// Si la key est valide, injecte le TenantId dans HttpContext.Items.
/// Complementaire au JWT - l un ou l autre suffit.
/// </summary>
public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeader = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IApiKeyService apiKeyService)
    {
        if (context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey))
        {
            var (isValid, tenantId) = apiKeyService.ValidateApiKey(apiKey!);
            if (isValid)
            {
                context.Items["TenantId"] = tenantId;
                context.Items["ApiKeyAuthenticated"] = true;
            }
        }

        await _next(context);
    }
}
'@

Set-Content -Path "src\Modules\Security\OneLine.Security.Infrastructure\SecurityExtensions.cs" -Encoding UTF8 -Value @'
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Security.Infrastructure.ApiKeys;
using OneLine.Security.Infrastructure.BruteForce;
using OneLine.Security.Infrastructure.Options;

namespace OneLine.Security.Infrastructure;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(SecurityOptions.SectionName);
        services.Configure<SecurityOptions>(opts =>
        {
            opts.MaxRequestsPerMinutePerIp = int.Parse(
                section["MaxRequestsPerMinutePerIp"] ?? "60");
            opts.MaxRequestsPerMinutePerUser = int.Parse(
                section["MaxRequestsPerMinutePerUser"] ?? "100");
            opts.MaxFailedLoginAttempts = int.Parse(
                section["MaxFailedLoginAttempts"] ?? "5");
            opts.LockoutDurationMinutes = int.Parse(
                section["LockoutDurationMinutes"] ?? "15");
        });

        services.AddMemoryCache();
        services.AddScoped<IBruteForceProtectionService, BruteForceProtectionService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();

        return services;
    }
}
'@

# ── OBSERVABILITY ────────────────────────────────────────────

Set-Content -Path "src\Modules\Observability\OneLine.Observability.Infrastructure\Logging\SerilogConfiguration.cs" -Encoding UTF8 -Value @'
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace OneLine.Observability.Infrastructure.Logging;

/// <summary>
/// Configuration de Serilog pour le logging structure.
///
/// Output :
///   Console : logs lisibles en developpement
///   File    : logs JSON en production (rotation quotidienne)
///
/// Enrichissements automatiques :
///   CorrelationId, TenantId, UserId sur chaque log
/// </summary>
public static class SerilogConfiguration
{
    public static IHostBuilder ConfigureSerilog(
        this IHostBuilder hostBuilder,
        IConfiguration? configuration = null)
    {
        return hostBuilder.UseSerilog((context, config) =>
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                   "{Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/oneline-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] " +
                                   "{Message:lj} {Properties:j}{NewLine}{Exception}",
                    retainedFileCountLimit: 7);
        });
    }
}
'@

Set-Content -Path "src\Modules\Observability\OneLine.Observability.Infrastructure\Middleware\CorrelationIdMiddleware.cs" -Encoding UTF8 -Value @'
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace OneLine.Observability.Infrastructure.Middleware;

/// <summary>
/// Middleware qui genere un X-Correlation-Id unique pour chaque requete.
///
/// Role :
///   - Permet de tracer une requete a travers tous les logs
///   - Retourne le CorrelationId dans la reponse HTTP
///   - Enrichit le contexte Serilog pour tous les logs de la requete
///
/// Usage client :
///   - Lire X-Correlation-Id dans la reponse
///   - Renvoyer dans les requetes suivantes pour le debugging
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Recuperer depuis le header ou generer un nouveau
        var correlationId = context.Request.Headers
            .TryGetValue(CorrelationIdHeader, out var existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString();

        // Ajouter a la reponse pour que le client puisse tracer
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Enrichir le contexte Serilog
        // Tous les logs dans cette requete auront CorrelationId
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        {
            await _next(context);
        }
    }
}
'@

Set-Content -Path "src\Modules\Observability\OneLine.Observability.Infrastructure\Middleware\RequestLoggingMiddleware.cs" -Encoding UTF8 -Value @'
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
'@

Set-Content -Path "src\Modules\Observability\OneLine.Observability.Infrastructure\Metrics\PrometheusSetup.cs" -Encoding UTF8 -Value @'
using Prometheus;

namespace OneLine.Observability.Infrastructure.Metrics;

/// <summary>
/// Configuration des metriques Prometheus.
///
/// Metriques exposees sur /metrics :
///   - http_requests_total : nombre de requetes par methode/path/status
///   - http_request_duration_seconds : duree des requetes
///   - active_tenants : nombre de tenants actifs
///
/// Compatible avec Grafana pour les dashboards.
/// </summary>
public static class PrometheusSetup
{
    // Compteur de requetes HTTP
    public static readonly Counter HttpRequestsTotal = Metrics
        .CreateCounter(
            "oneline_http_requests_total",
            "Nombre total de requetes HTTP",
            new CounterConfiguration
            {
                LabelNames = ["method", "path", "status"]
            });

    // Histogramme de duree des requetes
    public static readonly Histogram HttpRequestDuration = Metrics
        .CreateHistogram(
            "oneline_http_request_duration_seconds",
            "Duree des requetes HTTP en secondes",
            new HistogramConfiguration
            {
                LabelNames = ["method", "path"],
                Buckets = [0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5]
            });

    // Gauge du nombre de tenants actifs
    public static readonly Gauge ActiveTenants = Metrics
        .CreateGauge(
            "oneline_active_tenants",
            "Nombre de tenants actifs dans le systeme");
}
'@

Set-Content -Path "src\Modules\Observability\OneLine.Observability.Infrastructure\ObservabilityExtensions.cs" -Encoding UTF8 -Value @'
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Observability.Infrastructure.Middleware;
using Prometheus;

namespace OneLine.Observability.Infrastructure;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservabilityInfrastructure(
        this IServiceCollection services)
    {
        return services;
    }

    public static IApplicationBuilder UseObservability(
        this IApplicationBuilder app)
    {
        // Correlation ID sur chaque requete
        app.UseMiddleware<CorrelationIdMiddleware>();

        // Logging de chaque requete
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Endpoint Prometheus /metrics
        app.UseMetricServer();
        app.UseHttpMetrics();

        return app;
    }
}
'@

# ── MISE A JOUR appsettings.json ─────────────────────────────
Set-Content -Path "src\OneLine.API\appsettings.json" -Encoding UTF8 -Value @'
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=oneline_saaskit;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "SecretKey": "OneLine-SuperSecret-Key-2025-MinimumLength32Chars!",
    "Issuer": "OneLine.API",
    "Audience": "OneLine.Client",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "Stripe": {
    "SecretKey": "sk_test_YOUR_KEY_HERE",
    "PublishableKey": "pk_test_YOUR_KEY_HERE",
    "WebhookSecret": "whsec_YOUR_SECRET_HERE"
  },
  "Security": {
    "MaxRequestsPerMinutePerIp": 60,
    "MaxRequestsPerMinutePerUser": 100,
    "MaxFailedLoginAttempts": 5,
    "LockoutDurationMinutes": 15
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
'@

# ── MISE A JOUR Program.cs ───────────────────────────────────
Write-Host "`n[5/6] Mise a jour Program.cs..." -ForegroundColor Yellow

Set-Content -Path "src\OneLine.API\Program.cs" -Encoding UTF8 -Value @'
using OneLine.Auth.Application;
using OneLine.Auth.Infrastructure;
using OneLine.Billing.Application;
using OneLine.Billing.Application.UseCases.CreateSubscription;
using OneLine.Billing.Infrastructure;
using OneLine.Billing.Infrastructure.Middleware;
using OneLine.Observability.Infrastructure;
using OneLine.Observability.Infrastructure.Middleware;
using OneLine.Security.Infrastructure;
using OneLine.Security.Infrastructure.Middleware;
using OneLine.Tenants.Application;
using OneLine.Tenants.Infrastructure;
using OneLine.Tenants.Infrastructure.Middleware;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// ── Modules ──────────────────────────────────────────────────
builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddTenantsApplication();
builder.Services.AddTenantsInfrastructure(builder.Configuration);
builder.Services.AddBillingApplication();
builder.Services.AddBillingInfrastructure(builder.Configuration);
builder.Services.AddSecurityInfrastructure(builder.Configuration);
builder.Services.AddObservabilityInfrastructure();

// MediatR explicite pour Billing
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CreateSubscriptionCommand).Assembly));

// ── API ──────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ── Middleware Pipeline ───────────────────────────────────────
// Ordre important !
app.UseObservability();                          // CorrelationId + Logs + Metrics
app.UseMiddleware<RateLimitMiddleware>();         // Rate limiting par IP
app.UseMiddleware<ApiKeyMiddleware>();            // Auth par API Key
app.UseMiddleware<TenantMiddleware>();            // Resolution tenant
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SubscriptionMiddleware>();      // Check abonnement
app.MapControllers();

app.Run();
'@

Write-Host "Program.cs mis a jour" -ForegroundColor Green

# ── ETAPE 6 : Build ──────────────────────────────────────────
Write-Host "`n[6/6] Build..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== BUILD REUSSI ===" -ForegroundColor Green
    Write-Host "Lance l API :" -ForegroundColor Cyan
    Write-Host "dotnet run --project src\OneLine.API\OneLine.API.csproj" -ForegroundColor Gray
} else {
    Write-Host "`n=== BUILD ECHOUE - voir erreurs ci-dessus ===" -ForegroundColor Red
}
