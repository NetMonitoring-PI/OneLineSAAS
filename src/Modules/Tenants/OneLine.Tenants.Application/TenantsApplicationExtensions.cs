using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OneLine.Tenants.Application;

public static class TenantsApplicationExtensions
{
    public static IServiceCollection AddTenantsApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(TenantsApplicationExtensions).Assembly;

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
