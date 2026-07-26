using Microsoft.EntityFrameworkCore;
using OneLine.AI.Domain.Entities;

namespace OneLine.AI.Infrastructure.Persistence;

public sealed class AIDbContext : DbContext
{
    public AIDbContext(DbContextOptions<AIDbContext> options)
        : base(options) { }

    public DbSet<AIConversation> Conversations => Set<AIConversation>();
    public DbSet<AIMessage> Messages => Set<AIMessage>();
    public DbSet<AIUsage> Usages => Set<AIUsage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("ai");

        builder.Entity<AIConversation>(e =>
        {
            e.ToTable("conversations");
            e.HasKey(c => c.Id);
            e.Property(c => c.Title).HasMaxLength(200).IsRequired();
            e.HasIndex(c => c.TenantId)
             .HasDatabaseName("ix_conversations_tenant_id");
            e.HasMany(c => c.Messages)
             .WithOne()
             .HasForeignKey(m => m.ConversationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AIMessage>(e =>
        {
            e.ToTable("messages");
            e.HasKey(m => m.Id);
            e.Property(m => m.Content).IsRequired();
            e.Property(m => m.Role)
             .HasConversion<string>().HasMaxLength(20);
            e.HasIndex(m => m.ConversationId)
             .HasDatabaseName("ix_messages_conversation_id");
        });

        builder.Entity<AIUsage>(e =>
        {
            e.ToTable("usages");
            e.HasKey(u => u.Id);
            e.Property(u => u.Model).HasMaxLength(50).IsRequired();
            e.Property(u => u.Provider).HasMaxLength(20).IsRequired();
            e.Property(u => u.EstimatedCostUsd).HasPrecision(10, 6);
            e.Property(u => u.ConversationId).HasMaxLength(100);
            e.HasIndex(u => u.TenantId)
             .HasDatabaseName("ix_usages_tenant_id");
        });
    }
}
