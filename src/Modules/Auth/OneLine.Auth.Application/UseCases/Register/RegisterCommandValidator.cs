using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace OneLine.Auth.Application.UseCases.Register;

/// <summary>
/// Validation de la commande Register.
///
/// FluentValidation → validation déclarative et lisible.
/// Exécutée AVANT le Handler via le pipeline MediatR.
/// Si invalide → Result.Failure retourné automatiquement.
///
/// Pattern : Validation Pipeline Behavior
/// </summary>
public sealed class RegisterCommandValidator
    : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Le prénom est obligatoire.")
            .MaximumLength(50).WithMessage("Prénom trop long (max 50).");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(50).WithMessage("Nom trop long (max 50).");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire.")
            .EmailAddress().WithMessage("Format email invalide.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
            .MinimumLength(8).WithMessage("Minimum 8 caractères.")
            .Matches("[A-Z]").WithMessage("Au moins une majuscule.")
            .Matches("[0-9]").WithMessage("Au moins un chiffre.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Les mots de passe ne correspondent pas.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Le tenant est obligatoire.");
    }
}
