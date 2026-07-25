namespace OneLine.Tenants.Application.DTOs;

public sealed record TenantDto(
    Guid Id,
    string Name,
    string Subdomain,
    string Plan,
    string Status,
    string? ContactEmail,
    bool IsActive,
    DateTime? TrialEndsAt,
    DateTime CreatedAt
);
