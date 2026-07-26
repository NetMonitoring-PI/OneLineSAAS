using OneLine.AI.Domain.Enums;

namespace OneLine.AI.Infrastructure.Options;

public sealed class AIOptions
{
    public const string SectionName = "AI";
    public AIProvider Provider { get; set; } = AIProvider.OpenAI;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
    public string BaseUrl { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 2000;
    public float Temperature { get; set; } = 0.7f;
}
