using OneLine.Billing.Domain.Entities;

namespace OneLine.Billing.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByStripeInvoiceIdAsync(string stripeId, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(Invoice invoice, CancellationToken ct = default);
    void Update(Invoice invoice);
}
