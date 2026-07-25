using Microsoft.EntityFrameworkCore;
using OneLine.Tenants.Domain.Entities;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository : ITenantRepository
{
    private readonly TenantsDbContext _context;

    public TenantRepository(TenantsDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<Tenant?> GetBySubdomainAsync(
        string subdomain, CancellationToken ct = default)
        => await _context.Tenants
            .FirstOrDefaultAsync(
                t => t.Subdomain == subdomain.ToLowerInvariant(), ct);

    public async Task<bool> ExistsBySubdomainAsync(
        string subdomain, CancellationToken ct = default)
        => await _context.Tenants
            .AnyAsync(
                t => t.Subdomain == subdomain.ToLowerInvariant(), ct);

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(
        CancellationToken ct = default)
        => await _context.Tenants.ToListAsync(ct);

    public async Task AddAsync(
        Tenant tenant, CancellationToken ct = default)
        => await _context.Tenants.AddAsync(tenant, ct);

    public void Update(Tenant tenant)
        => _context.Tenants.Update(tenant);

    public void Delete(Tenant tenant)
        => _context.Tenants.Remove(tenant);
}
