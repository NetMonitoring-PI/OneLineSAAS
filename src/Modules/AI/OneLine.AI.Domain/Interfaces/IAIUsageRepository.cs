using OneLine.AI.Domain.Entities;

namespace OneLine.AI.Domain.Interfaces;

public interface IAIUsageRepository
{
    Task AddAsync(AIUsage usage, CancellationToken ct = default);
    Task<int> GetMonthlyTokensAsync(Guid tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<AIUsage>> GetByTenantIdAsync(
        Guid tenantId, int limit = 50, CancellationToken ct = default);
}
