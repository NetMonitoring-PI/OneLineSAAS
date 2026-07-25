using FluentValidation;

namespace OneLine.Tenants.Application.UseCases.CreateTenant;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom du tenant est obligatoire.")
            .MaximumLength(100);

        RuleFor(x => x.Subdomain)
            .NotEmpty().WithMessage("Le sous-domaine est obligatoire.")
            .MaximumLength(50)
            .Matches("^[a-z0-9-]+$").WithMessage("Sous-domaine: lettres minuscules, chiffres et tirets.");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Email invalide.")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));
    }
}
