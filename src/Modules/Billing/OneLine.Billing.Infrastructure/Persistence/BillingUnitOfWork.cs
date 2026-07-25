using OneLine.Billing.Application.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence;

public sealed class BillingUnitOfWork : IUnitOfWork
{
    private readonly BillingDbContext _context;
    public BillingUnitOfWork(BillingDbContext context) => _context = context;
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
