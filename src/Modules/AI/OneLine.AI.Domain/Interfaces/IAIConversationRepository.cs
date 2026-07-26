using OneLine.AI.Domain.Entities;

namespace OneLine.AI.Domain.Interfaces;

public interface IAIConversationRepository
{
    Task<AIConversation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AIConversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AIConversation>> GetByTenantIdAsync(
        Guid tenantId, CancellationToken ct = default);
    Task AddAsync(AIConversation conversation, CancellationToken ct = default);
    void Update(AIConversation conversation);
}
