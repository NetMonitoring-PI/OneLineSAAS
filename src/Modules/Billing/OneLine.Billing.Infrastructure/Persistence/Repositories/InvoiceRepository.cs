using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _context;
    public InvoiceRepository(BillingDbContext context) => _context = context;

    public async Task<Invoice?> GetByStripeInvoiceIdAsync(string stripeId, CancellationToken ct = default)
        => await _context.Invoices.FirstOrDefaultAsync(i => i.StripeInvoiceId == stripeId, ct);

    public async Task<IReadOnlyList<Invoice>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Invoices.Where(i => i.TenantId == tenantId).ToListAsync(ct);

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
        => await _context.Invoices.AddAsync(invoice, ct);

    public void Update(Invoice invoice) => _context.Invoices.Update(invoice);
}
