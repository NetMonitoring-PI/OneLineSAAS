using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence.Repositories;

public sealed class PlanRepository : IPlanRepository
{
    private readonly BillingDbContext _context;
    public PlanRepository(BillingDbContext context) => _context = context;

    public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Plans.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.Plans.Where(p => p.IsActive).ToListAsync(ct);

    public async Task AddAsync(Plan plan, CancellationToken ct = default)
        => await _context.Plans.AddAsync(plan, ct);

    public void Update(Plan plan) => _context.Plans.Update(plan);
}
