using MediatR;
using OneLine.Billing.Application.Interfaces;
using OneLine.Billing.Domain.Errors;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CancelSubscription;

public sealed class CancelSubscriptionCommandHandler
    : IRequestHandler<CancelSubscriptionCommand, Result>
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepo,
        IStripeService stripeService,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepo = subscriptionRepo;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CancelSubscriptionCommand command, CancellationToken ct)
    {
        var sub = await _subscriptionRepo.GetByTenantIdAsync(command.TenantId, ct);
        if (sub is null) return BillingErrors.SubscriptionNotFound;

        // Annuler dans Stripe si connectÃ©
        if (!string.IsNullOrEmpty(sub.StripeSubscriptionId))
            await _stripeService.CancelSubscriptionAsync(sub.StripeSubscriptionId, ct);

        sub.Cancel();
        _subscriptionRepo.Update(sub);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
