using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Billing.Domain.Errors;
using OneLine.Billing.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.GetSubscription;

public sealed class GetSubscriptionByTenantQueryHandler
    : IRequestHandler<GetSubscriptionByTenantQuery, Result<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepo;
    private readonly IPlanRepository _planRepo;

    public GetSubscriptionByTenantQueryHandler(
        ISubscriptionRepository subscriptionRepo,
        IPlanRepository planRepo)
    {
        _subscriptionRepo = subscriptionRepo;
        _planRepo = planRepo;
    }

    public async Task<Result<SubscriptionDto>> Handle(
        GetSubscriptionByTenantQuery query, CancellationToken ct)
    {
        var sub = await _subscriptionRepo.GetByTenantIdAsync(query.TenantId, ct);
        if (sub is null) return BillingErrors.SubscriptionNotFound;

        var plan = await _planRepo.GetByIdAsync(sub.PlanId, ct);

        return new SubscriptionDto(
            sub.Id, sub.TenantId, sub.PlanId,
            plan?.Name ?? "Unknown",
            sub.Status.ToString(), sub.IsActive,
            plan?.Price ?? 0, plan?.Interval.ToString() ?? "",
            sub.CurrentPeriodEnd, sub.TrialEndsAt,
            sub.CreatedAt);
    }
}
