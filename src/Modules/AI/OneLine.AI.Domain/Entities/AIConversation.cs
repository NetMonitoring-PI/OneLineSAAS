using OneLine.AI.Domain.Enums;

namespace OneLine.AI.Domain.Entities;

public sealed class AIConversation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int TotalTokensUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<AIMessage> Messages { get; set; } = [];
}
