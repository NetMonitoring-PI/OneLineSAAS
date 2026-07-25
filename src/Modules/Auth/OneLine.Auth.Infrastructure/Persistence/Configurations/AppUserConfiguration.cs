using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneLine.Auth.Domain.Entities;

namespace OneLine.Auth.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour AppUser.
///
/// Pattern : IEntityTypeConfiguration<T>
/// → Séparation des configurations de mapping
/// → Plus propre que tout mettre dans OnModelCreating
/// → Une classe par entité
/// </summary>
public sealed class AppUserConfiguration
    : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        // Nom de la table
        builder.ToTable("users");

        // Propriétés
        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.TenantId)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion<string>()  // stocké comme string en DB
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        // Index sur TenantId → requêtes multi-tenant rapides
        builder.HasIndex(u => u.TenantId)
            .HasDatabaseName("ix_users_tenant_id");

        // Index sur Email → login rapide
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email");

        // Relation : un User → plusieurs RefreshTokens
        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Cascade → supprimer user = supprimer ses tokens
    }
}