using FluentValidation;

namespace OneLine.Tenants.Application.UseCases.CreateTenant;

public sealed class CreateTenantCommandValidator
    : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom du tenant est obligatoire.")
            .MaximumLength(100).WithMessage("Nom trop long (max 100).");

        RuleFor(x => x.Subdomain)
            .NotEmpty().WithMessage("Le sous-domaine est obligatoire.")
            .MaximumLength(50).WithMessage("Sous-domaine trop long (max 50).")
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Sous-domaine : uniquement lettres minuscules, chiffres et tirets.");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Email invalide.")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));
    }
}
