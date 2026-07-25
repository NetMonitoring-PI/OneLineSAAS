using OneLine.Billing.Domain.Entities;

namespace OneLine.Billing.Domain.Interfaces;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Plan plan, CancellationToken ct = default);
    void Update(Plan plan);
}
