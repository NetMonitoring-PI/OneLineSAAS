using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Errors;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CreateSubscription;

/// <summary>
/// Flow complet de crÃ©ation d abonnement :
/// 1. VÃ©rifier pas dÃ©jÃ  abonnÃ©
/// 2. RÃ©cupÃ©rer le plan
/// 3. CrÃ©er le Customer Stripe
/// 4. CrÃ©er la Subscription Stripe
/// 5. Sauvegarder en DB
/// </summary>
public sealed class CreateSubscriptionCommandHandler
    : IRequestHandler<CreateSubscriptionCommand, Result<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IPlanRepository _planRepo;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepo,
        IPlanRepository planRepo,
        IStripeService stripeService,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepo = subscriptionRepo;
        _planRepo = planRepo;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SubscriptionDto>> Handle(
        CreateSubscriptionCommand command, CancellationToken ct)
    {
        // 1. VÃ©rifier pas dÃ©jÃ  abonnÃ©
        var hasActive = await _subscriptionRepo
            .HasActiveSubscriptionAsync(command.TenantId, ct);
        if (hasActive) return BillingErrors.AlreadySubscribed;

        // 2. RÃ©cupÃ©rer le plan
        var plan = await _planRepo.GetByIdAsync(command.PlanId, ct);
        if (plan is null) return BillingErrors.PlanNotFound;

        // 3. CrÃ©er Customer Stripe
        var customerId = await _stripeService.CreateCustomerAsync(
            command.TenantEmail, command.TenantName, ct);

        // 4. CrÃ©er Subscription en DB (Trialing)
        var subscription = Subscription.Create(
            command.TenantId, command.PlanId, customerId);

        // 5. Si plan Stripe configurÃ© â†’ crÃ©er abonnement Stripe
        if (!string.IsNullOrEmpty(plan.StripePriceId))
        {
            var (stripeSubId, periodEnd) = await _stripeService
                .CreateSubscriptionAsync(customerId, plan.StripePriceId, ct);

            subscription.Activate(periodEnd, stripeSubId);
        }

        await _subscriptionRepo.AddAsync(subscription, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new SubscriptionDto(
            subscription.Id, subscription.TenantId, subscription.PlanId,
            plan.Name, subscription.Status.ToString(), subscription.IsActive,
            plan.Price, plan.Interval.ToString(),
            subscription.CurrentPeriodEnd, subscription.TrialEndsAt,
            subscription.CreatedAt);
    }
}
