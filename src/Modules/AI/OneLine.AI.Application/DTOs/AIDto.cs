namespace OneLine.AI.Application.DTOs;

public sealed record ChatResponseDto(
    Guid ConversationId,
    string Content,
    int TokensUsed,
    int MonthlyTokensUsed,
    int MonthlyQuota,
    string Model,
    string Provider
);

public sealed record ConversationDto(
    Guid Id,
    Guid TenantId,
    string Title,
    bool IsActive,
    int TotalTokensUsed,
    int MessageCount,
    DateTime CreatedAt
);

public sealed record AIUsageDto(
    int MonthlyTokensUsed,
    int MonthlyQuota,
    int RemainingTokens,
    decimal EstimatedCostUsd,
    bool IsQuotaExceeded
);
