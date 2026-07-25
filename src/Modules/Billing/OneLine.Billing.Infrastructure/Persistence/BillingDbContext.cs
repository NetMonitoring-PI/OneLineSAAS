using Microsoft.EntityFrameworkCore;
using OneLine.Billing.Domain.Entities;

namespace OneLine.Billing.Infrastructure.Persistence;

public sealed class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options)
        : base(options) { }

    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("billing");

        builder.Entity<Plan>(e =>
        {
            e.ToTable("plans");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(50).IsRequired();
            e.Property(p => p.Description).HasMaxLength(200);
            e.Property(p => p.Price).HasPrecision(10, 2).IsRequired();
            e.Property(p => p.Interval).HasConversion<string>().HasMaxLength(10);
            e.Property(p => p.StripeProductId).HasMaxLength(100);
            e.Property(p => p.StripePriceId).HasMaxLength(100);
        });

        builder.Entity<Subscription>(e =>
        {
            e.ToTable("subscriptions");
            e.HasKey(s => s.Id);
            e.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(s => s.StripeSubscriptionId).HasMaxLength(100);
            e.Property(s => s.StripeCustomerId).HasMaxLength(100);
            e.HasIndex(s => s.TenantId).HasDatabaseName("ix_subscriptions_tenant_id");
            e.HasIndex(s => s.StripeSubscriptionId).HasDatabaseName("ix_subscriptions_stripe_id");
            e.Ignore(s => s.DomainEvents);
            e.HasOne(s => s.Plan).WithMany().HasForeignKey(s => s.PlanId);
        });

        builder.Entity<Invoice>(e =>
        {
            e.ToTable("invoices");
            e.HasKey(i => i.Id);
            e.Property(i => i.Amount).HasPrecision(10, 2).IsRequired();
            e.Property(i => i.Currency).HasMaxLength(3);
            e.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(i => i.StripeInvoiceId).HasMaxLength(100);
            e.Property(i => i.StripeHostedUrl).HasMaxLength(500);
            e.HasIndex(i => i.TenantId).HasDatabaseName("ix_invoices_tenant_id");
        });
    }
}
