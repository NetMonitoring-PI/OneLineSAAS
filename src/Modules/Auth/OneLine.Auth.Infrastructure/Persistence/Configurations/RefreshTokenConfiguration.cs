using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneLine.Auth.Domain.Entities;

namespace OneLine.Auth.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.ExpiresAt)
            .IsRequired();

        builder.Property(t => t.RevokedReason)
            .HasMaxLength(200);

        builder.Property(t => t.ReplacedByToken)
            .HasMaxLength(200);

        builder.Property(t => t.CreatedByIp)
            .HasMaxLength(45); // IPv6 max = 45 chars

        // Index pour lookup rapide par token
        builder.HasIndex(t => t.Token)
            .IsUnique()
            .HasDatabaseName("ix_refresh_tokens_token");

        // Index pour trouver tous les tokens d'un user
        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("ix_refresh_tokens_user_id");
    }
}