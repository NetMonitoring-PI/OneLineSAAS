using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Auth.Application.Behaviors;

namespace OneLine.Auth.Application;

/// <summary>
/// Point d'entrée du module Application.
/// Une seule ligne dans Program.cs pour tout enregistrer.
///
/// Pattern : Extension Method + Module Registration
/// </summary>
public static class AuthApplicationExtensions
{
    public static IServiceCollection AddAuthApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(AuthApplicationExtensions).Assembly;

        // MediatR — découverte automatique des Handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Ajouter le pipeline de validation
            cfg.AddBehavior(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
        });

        // FluentValidation — découverte automatique des Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
