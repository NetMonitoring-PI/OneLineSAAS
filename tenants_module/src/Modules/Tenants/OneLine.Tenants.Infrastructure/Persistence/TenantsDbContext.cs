using Microsoft.EntityFrameworkCore;
using OneLine.Tenants.Domain.Entities;

namespace OneLine.Tenants.Infrastructure.Persistence;

public sealed class TenantsDbContext : DbContext
{
    public TenantsDbContext(DbContextOptions<TenantsDbContext> options)
        : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("tenants");

        builder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name)
                .HasMaxLength(100).IsRequired();

            entity.Property(t => t.Subdomain)
                .HasMaxLength(50).IsRequired();

            entity.Property(t => t.Plan)
                .HasConversion<string>()
                .HasMaxLength(20).IsRequired();

            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20).IsRequired();

            entity.Property(t => t.ContactEmail)
                .HasMaxLength(200);

            entity.Property(t => t.Description)
                .HasMaxLength(500);

            entity.HasIndex(t => t.Subdomain)
                .IsUnique()
                .HasDatabaseName("ix_tenants_subdomain");

            // Ignore les domain events — pas stockés en DB
            entity.Ignore(t => t.DomainEvents);
        });
    }
}
