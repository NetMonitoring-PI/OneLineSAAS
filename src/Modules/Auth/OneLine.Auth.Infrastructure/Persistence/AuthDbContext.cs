using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OneLine.Auth.Domain.Entities;

namespace OneLine.Auth.Infrastructure.Persistence;

public sealed class AuthDbContext
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) { }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("auth");

        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.Property(u => u.FirstName)
                .HasMaxLength(50).IsRequired();
            entity.Property(u => u.LastName)
                .HasMaxLength(50).IsRequired();
            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20).IsRequired();
            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);
            entity.HasIndex(u => u.TenantId)
                .HasDatabaseName("ix_users_tenant_id");
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Token)
                .HasMaxLength(200).IsRequired();
            entity.Property(t => t.RevokedReason)
                .HasMaxLength(200);
            entity.Property(t => t.ReplacedByToken)
                .HasMaxLength(200);
            entity.Property(t => t.CreatedByIp)
                .HasMaxLength(45);
            entity.HasIndex(t => t.Token)
                .IsUnique()
                .HasDatabaseName("ix_refresh_tokens_token");
        });
    }
}
