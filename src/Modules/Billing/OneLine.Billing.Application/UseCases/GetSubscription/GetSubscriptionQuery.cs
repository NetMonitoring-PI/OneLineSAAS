using MediatR;
using OneLine.Billing.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.GetSubscription;

public sealed record GetSubscriptionByTenantQuery(Guid TenantId)
    : IRequest<Result<SubscriptionDto>>;
