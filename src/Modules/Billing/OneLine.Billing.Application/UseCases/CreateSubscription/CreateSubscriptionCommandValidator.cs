using FluentValidation;

namespace OneLine.Billing.Application.UseCases.CreateSubscription;

public sealed class CreateSubscriptionCommandValidator
    : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.TenantEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(100);
    }
}
