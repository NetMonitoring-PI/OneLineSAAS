using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;
using OneLine.Shared.Domain.Result;

namespace OneLine.Auth.Application.Behaviors;

/// <summary>
/// Pipeline MediatR — s'exécute AVANT chaque Handler.
///
/// Rôle : valider automatiquement toutes les Commands/Queries
/// via FluentValidation avant d'atteindre le Handler.
///
/// Pattern : Pipeline Behavior (Decorator sur MediatR)
///
/// Avantage :
/// → Les Handlers n'ont pas besoin de valider manuellement
/// → Validation centralisée et automatique
/// → Si invalide → Result.Failure retourné sans appeler Handler
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // Si aucun validator → on passe directement au Handler
        if (!_validators.Any())
            return await next();

        // Exécuter tous les validators
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        // Si validation OK → passer au Handler
        if (failures.Count == 0)
            return await next();

        // Si échec → construire le message d'erreur
        var errorMessage = string.Join("; ",
            failures.Select(f => f.ErrorMessage));

        // Retourner Result.Failure sans appeler le Handler
        var error = Error.Validation(
            "Validation.Failed",
            errorMessage);

        // On doit créer le bon type de Result dynamiquement
        var resultType = typeof(TResponse);

        if (resultType.IsGenericType &&
            resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(Result<object>.Failure))!;

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        return await next();
    }
}
