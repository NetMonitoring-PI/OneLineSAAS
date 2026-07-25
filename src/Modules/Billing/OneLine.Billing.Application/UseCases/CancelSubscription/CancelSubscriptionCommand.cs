using MediatR;
using OneLine.Shared.Domain.Result;

namespace OneLine.Billing.Application.UseCases.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid TenantId)
    : IRequest<Result>;
