using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace OneLine.Auth.Application.UseCases.Login;

public sealed class LoginCommandValidator
    : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire.")
            .EmailAddress().WithMessage("Format email invalide.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.");
    }
}
