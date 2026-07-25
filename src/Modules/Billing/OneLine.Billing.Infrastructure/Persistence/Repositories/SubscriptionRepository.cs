using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;
using OneLine.Billing.Domain.Enums;
using OneLine.Billing.Domain.Interfaces;

namespace OneLine.Billing.Infrastructure.Persistence.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly BillingDbContext _context;
    public SubscriptionRepository(BillingDbContext context) => _context = context;

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Subscriptions.Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Subscription?> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Subscriptions.Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.TenantId == tenantId, ct);

    public async Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeId, CancellationToken ct = default)
        => await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeId, ct);

    public async Task<bool> HasActiveSubscriptionAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Subscriptions.AnyAsync(
            s => s.TenantId == tenantId &&
                (s.Status == SubscriptionStatus.Active ||
                 s.Status == SubscriptionStatus.Trialing), ct);

    public async Task AddAsync(Subscription subscription, CancellationToken ct = default)
        => await _context.Subscriptions.AddAsync(subscription, ct);

    public void Update(Subscription subscription)
        => _context.Subscriptions.Update(subscription);
}
