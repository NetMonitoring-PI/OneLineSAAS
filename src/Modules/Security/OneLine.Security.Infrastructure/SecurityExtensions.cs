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
