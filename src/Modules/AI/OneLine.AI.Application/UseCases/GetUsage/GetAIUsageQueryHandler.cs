using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.AI.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.GetUsage;

public sealed class GetAIUsageQueryHandler
    : IRequestHandler<GetAIUsageQuery, Result<AIUsageDto>>
{
    private readonly IAIUsageRepository _usageRepo;
    private const int DefaultMonthlyQuota = 50_000;

    public GetAIUsageQueryHandler(IAIUsageRepository usageRepo)
    {
        _usageRepo = usageRepo;
    }

    public async Task<Result<AIUsageDto>> Handle(
        GetAIUsageQuery query, CancellationToken ct)
    {
        var monthlyTokens = await _usageRepo
            .GetMonthlyTokensAsync(query.TenantId, ct);

        var usages = await _usageRepo
            .GetByTenantIdAsync(query.TenantId, 50, ct);

        var totalCost = usages.Sum(u => u.EstimatedCostUsd);

        return new AIUsageDto(
            MonthlyTokensUsed: monthlyTokens,
            MonthlyQuota: DefaultMonthlyQuota,
            RemainingTokens: Math.Max(0, DefaultMonthlyQuota - monthlyTokens),
            EstimatedCostUsd: totalCost,
            IsQuotaExceeded: monthlyTokens >= DefaultMonthlyQuota);
    }
}
