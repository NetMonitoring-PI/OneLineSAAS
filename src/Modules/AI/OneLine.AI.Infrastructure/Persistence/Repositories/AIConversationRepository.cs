using Microsoft.EntityFrameworkCore;
using OneLine.AI.Domain.Entities;
using OneLine.AI.Domain.Interfaces;

namespace OneLine.AI.Infrastructure.Persistence.Repositories;

public sealed class AIConversationRepository : IAIConversationRepository
{
    private readonly AIDbContext _context;
    public AIConversationRepository(AIDbContext context) => _context = context;

    public async Task<AIConversation?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<AIConversation?> GetByIdWithMessagesAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<AIConversation>> GetByTenantIdAsync(
        Guid tenantId, CancellationToken ct = default)
        => await _context.Conversations
            .Where(c => c.TenantId == tenantId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(
        AIConversation conversation, CancellationToken ct = default)
        => await _context.Conversations.AddAsync(conversation, ct);

    public void Update(AIConversation conversation)
        => _context.Conversations.Update(conversation);
}
