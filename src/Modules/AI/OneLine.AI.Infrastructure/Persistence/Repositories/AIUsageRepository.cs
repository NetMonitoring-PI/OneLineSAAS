using Microsoft.EntityFrameworkCore;
using OneLine.AI.Domain.Entities;
using OneLine.AI.Domain.Interfaces;

namespace OneLine.AI.Infrastructure.Persistence.Repositories;

public sealed class AIUsageRepository : IAIUsageRepository
{
    private readonly AIDbContext _context;
    public AIUsageRepository(AIDbContext context) => _context = context;

    public async Task AddAsync(AIUsage usage, CancellationToken ct = default)
    {
        _context.Usages.Add(usage);
        await Task.CompletedTask;
    }

    public async Task<int> GetMonthlyTokensAsync(
        Guid tenantId, CancellationToken ct = default)
    {
        var firstDay = new DateTime(
            DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1,
            0, 0, 0, DateTimeKind.Utc);

        return await _context.Usages
            .Where(u => u.TenantId == tenantId && u.CreatedAt >= firstDay)
            .SumAsync(u => u.TotalTokens, ct);
    }

    public async Task<IReadOnlyList<AIUsage>> GetByTenantIdAsync(
        Guid tenantId, int limit = 50, CancellationToken ct = default)
        => await _context.Usages
            .Where(u => u.TenantId == tenantId)
            .OrderByDescending(u => u.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
}
