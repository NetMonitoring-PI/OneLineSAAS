using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Auth.Application.Behaviors;

namespace OneLine.Tenants.Application;

public static class TenantsApplicationExtensions
{
    public static IServiceCollection AddTenantsApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(TenantsApplicationExtensions).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
