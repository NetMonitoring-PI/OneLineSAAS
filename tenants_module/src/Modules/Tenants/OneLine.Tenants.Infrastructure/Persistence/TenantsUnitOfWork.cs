using OneLine.Tenants.Application.Interfaces;

namespace OneLine.Tenants.Infrastructure.Persistence;

public sealed class TenantsUnitOfWork : IUnitOfWork
{
    private readonly TenantsDbContext _context;

    public TenantsUnitOfWork(TenantsDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
