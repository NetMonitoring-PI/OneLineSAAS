namespace OneLine.AI.Domain.Entities;

public sealed class AIUsage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public decimal EstimatedCostUsd { get; set; }
    public string? ConversationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static AIUsage Create(
        Guid tenantId,
        int promptTokens,
        int completionTokens,
        string model,
        string provider,
        string? conversationId = null)
    {
        var totalTokens = promptTokens + completionTokens;
        var costPer1k = model.Contains("gpt-4") ? 0.03m :
                        model.Contains("gpt-3.5") ? 0.002m : 0.001m;

        return new AIUsage
        {
            TenantId = tenantId,
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            TotalTokens = totalTokens,
            Model = model,
            Provider = provider,
            EstimatedCostUsd = (totalTokens / 1000m) * costPer1k,
            ConversationId = conversationId
        };
    }
}
