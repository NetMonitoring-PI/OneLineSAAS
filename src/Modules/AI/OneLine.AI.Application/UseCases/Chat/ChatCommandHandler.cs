using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.AI.Application.Interfaces;
using OneLine.AI.Domain.Entities;
using OneLine.AI.Domain.Enums;
using OneLine.AI.Domain.Errors;
using OneLine.AI.Domain.Interfaces;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.Chat;

public sealed class ChatCommandHandler
    : IRequestHandler<ChatCommand, Result<ChatResponseDto>>
{
    private readonly ILLMService _llmService;
    private readonly IAIConversationRepository _conversationRepo;
    private readonly IAIUsageRepository _usageRepo;
    private readonly IUnitOfWork _unitOfWork;
    private const int DefaultMonthlyQuota = 50_000;

    public ChatCommandHandler(
        ILLMService llmService,
        IAIConversationRepository conversationRepo,
        IAIUsageRepository usageRepo,
        IUnitOfWork unitOfWork)
    {
        _llmService = llmService;
        _conversationRepo = conversationRepo;
        _usageRepo = usageRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChatResponseDto>> Handle(
        ChatCommand command, CancellationToken ct)
    {
        // 1. Verifier quota
        var monthlyTokens = await _usageRepo
            .GetMonthlyTokensAsync(command.TenantId, ct);
        if (monthlyTokens >= DefaultMonthlyQuota)
            return AIErrors.QuotaExceeded;

        // 2. Appel LLM en premier
        var messages = new List<LLMMessage>
        {
            new("system", string.IsNullOrEmpty(command.SystemPrompt)
                ? "Tu es un assistant IA utile et concis."
                : command.SystemPrompt),
            new("user", command.Message)
        };

        LLMResponse llmResponse;
        try
        {
            llmResponse = await _llmService.ChatAsync(messages, ct: ct);
        }
        catch (Exception)
        {
            return AIErrors.ProviderError;
        }

        // 3. Creer et sauvegarder tout en une seule transaction
        var conversation = new AIConversation
        {
            TenantId = command.TenantId,
            UserId = command.UserId,
            Title = command.Message.Length > 50
                ? command.Message[..50] + "..."
                : command.Message,
            TotalTokensUsed = llmResponse.TotalTokens,
            Messages =
            [
                new AIMessage
                {
                    Role = MessageRole.User,
                    Content = command.Message
                },
                new AIMessage
                {
                    Role = MessageRole.Assistant,
                    Content = llmResponse.Content,
                    TokenCount = llmResponse.TotalTokens
                }
            ]
        };

        var usage = new AIUsage
        {
            TenantId = command.TenantId,
            PromptTokens = llmResponse.PromptTokens,
            CompletionTokens = llmResponse.CompletionTokens,
            TotalTokens = llmResponse.TotalTokens,
            Model = llmResponse.Model,
            Provider = llmResponse.Provider,
            EstimatedCostUsd = (llmResponse.TotalTokens / 1000m) * 0.001m,
            ConversationId = conversation.Id.ToString()
        };

        await _conversationRepo.AddAsync(conversation, ct);
        await _usageRepo.AddAsync(usage, ct);

        // Une seule SaveChanges
        await _unitOfWork.SaveChangesAsync(ct);

        return new ChatResponseDto(
            ConversationId: conversation.Id,
            Content: llmResponse.Content,
            TokensUsed: llmResponse.TotalTokens,
            MonthlyTokensUsed: monthlyTokens + llmResponse.TotalTokens,
            MonthlyQuota: DefaultMonthlyQuota,
            Model: llmResponse.Model,
            Provider: llmResponse.Provider);
    }
}
