using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OneLine.AI.Application.UseCases.Chat;
using OneLine.AI.Application.UseCases.GetUsage;

namespace OneLine.AI.Application;

public static class AIApplicationExtensions
{
    public static IServiceCollection AddAIApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(AIApplicationExtensions).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Enregistrement explicite des handlers
            cfg.RegisterServicesFromAssemblyContaining<ChatCommandHandler>();
            cfg.RegisterServicesFromAssemblyContaining<GetAIUsageQueryHandler>();
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
