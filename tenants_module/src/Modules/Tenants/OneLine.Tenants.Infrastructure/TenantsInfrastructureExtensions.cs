using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Shared.Domain.Interfaces;
using OneLine.Tenants.Application.Interfaces;
using OneLine.Tenants.Domain.Interfaces;
using OneLine.Tenants.Infrastructure.Middleware;
using OneLine.Tenants.Infrastructure.Persistence;
using OneLine.Tenants.Infrastructure.Persistence.Repositories;
using OneLine.Tenants.Infrastructure.Resolvers;
using OneLine.Tenants.Infrastructure.Services;

namespace OneLine.Tenants.Infrastructure;

public static class TenantsInfrastructureExtensions
{
    public static IServiceCollection AddTenantsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── DbContext ────────────────────────────────────────
        services.AddDbContext<TenantsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // ── Repository + UnitOfWork ──────────────────────────
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUnitOfWork, TenantsUnitOfWork>();

        // ── Resolvers (Pattern Strategy) ─────────────────────
        // Tous enregistrés comme ITenantResolver
        // → injectés comme IEnumerable<ITenantResolver> dans le middleware
        services.AddScoped<ITenantResolver, HeaderTenantResolver>();
        services.AddScoped<ITenantResolver, ClaimTenantResolver>();
        services.AddScoped<ITenantResolver, SubdomainTenantResolver>();

        // ── Current Tenant Service ───────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentTenant, CurrentTenantService>();

        return services;
    }

    /// <summary>
    /// Enregistre le TenantMiddleware dans le pipeline HTTP.
    /// À appeler dans Program.cs AVANT UseAuthentication.
    /// </summary>
    public static IApplicationBuilder UseTenantsMiddleware(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<TenantMiddleware>();
        return app;
    }
}
