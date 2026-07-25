namespace OneLine.Billing.Application.DTOs;

public sealed record SubscriptionDto(
    Guid Id,
    Guid TenantId,
    Guid PlanId,
    string PlanName,
    string Status,
    bool IsActive,
    decimal Price,
    string Interval,
    DateTime CurrentPeriodEnd,
    DateTime? TrialEndsAt,
    DateTime CreatedAt
);

public sealed record PlanDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Interval,
    int TokenQuota,
    bool IsActive
);

public sealed record InvoiceDto(
    Guid Id,
    Guid TenantId,
    decimal Amount,
    string Currency,
    string Status,
    DateTime? PaidAt,
    string? StripeHostedUrl,
    DateTime CreatedAt
);
