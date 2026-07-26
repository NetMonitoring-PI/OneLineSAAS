using FluentValidation;

namespace OneLine.AI.Application.UseCases.Chat;

public sealed class ChatCommandValidator : AbstractValidator<ChatCommand>
{
    public ChatCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId est obligatoire.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Le message ne peut pas etre vide.")
            .MaximumLength(10000).WithMessage("Message trop long (max 10000 chars).");
    }
}
