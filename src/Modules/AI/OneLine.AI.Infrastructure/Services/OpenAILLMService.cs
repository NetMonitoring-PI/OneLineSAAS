using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OneLine.AI.Application.Interfaces;
using OneLine.AI.Infrastructure.Options;
using OpenAI.Chat;
using System.ClientModel;

namespace OneLine.AI.Infrastructure.Services;

/// <summary>
/// Implementation OpenAI du service LLM.
/// Pattern : Strategy (via ILLMService)
/// </summary>
public sealed class OpenAILLMService : ILLMService
{
    private readonly AIOptions _options;

    public string ProviderName => "OpenAI";
    public string DefaultModel => _options.Model;

    public OpenAILLMService(IOptions<AIOptions> options)
    {
        _options = options.Value;
    }

    public async Task<LLMResponse> ChatAsync(
        IReadOnlyList<LLMMessage> messages,
        string? model = null,
        CancellationToken ct = default)
    {
        var credential = new ApiKeyCredential(_options.ApiKey);
        var client = new AzureOpenAIClient(
            new Uri("https://api.openai.com/v1"),
            credential);

        var chatClient = client.GetChatClient(model ?? _options.Model);

        var chatMessages = messages.Select<LLMMessage, ChatMessage>(m =>
            m.Role switch
            {
                "system"    => ChatMessage.CreateSystemMessage(m.Content),
                "assistant" => ChatMessage.CreateAssistantMessage(m.Content),
                _           => ChatMessage.CreateUserMessage(m.Content)
            }).ToList();

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = _options.MaxTokens,
            Temperature = _options.Temperature
        };

        var completion = await chatClient
            .CompleteChatAsync(chatMessages, options, ct);

        var content = completion.Value.Content[0].Text;
        var usage = completion.Value.Usage;

        return new LLMResponse(
            Content: content,
            PromptTokens: usage.InputTokenCount,
            CompletionTokens: usage.OutputTokenCount,
            TotalTokens: usage.TotalTokenCount,
            Model: model ?? _options.Model,
            Provider: ProviderName);
    }
}
