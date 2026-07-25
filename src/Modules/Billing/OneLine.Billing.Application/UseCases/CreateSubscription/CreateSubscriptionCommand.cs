using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CreateSubscription;

public sealed record CreateSubscriptionCommand(
    Guid TenantId,
    Guid PlanId,
    string TenantEmail,
    string TenantName
) : IRequest<Result<SubscriptionDto>>;
