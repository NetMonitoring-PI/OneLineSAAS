using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OneLine.Billing.Application;

public static class BillingApplicationExtensions
{
    public static IServiceCollection AddBillingApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(BillingApplicationExtensions).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
