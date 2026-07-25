using OneLine.Tenants.Domain.Entities;

namespace OneLine.Tenants.Domain.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken ct = default);
    Task<bool> ExistsBySubdomainAsync(string subdomain, CancellationToken ct = default);
    Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
    void Update(Tenant tenant);
    void Delete(Tenant tenant);
}
