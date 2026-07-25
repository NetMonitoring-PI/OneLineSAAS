using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Shared.Domain.Interfaces;
using OneLine.Tenants.Application.Interfaces;
using OneLine.Tenants.Domain.Interfaces;
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
        services.AddDbContext<TenantsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUnitOfWork, TenantsUnitOfWork>();

        services.AddScoped<ITenantResolver, HeaderTenantResolver>();
        services.AddScoped<ITenantResolver, ClaimTenantResolver>();
        services.AddScoped<ITenantResolver, SubdomainTenantResolver>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentTenant, CurrentTenantService>();

        return services;
    }
}
