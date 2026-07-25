using Microsoft.AspNetCore.Http;
using OneLine.Shared.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Services;

/// <summary>
/// Implémentation de ICurrentTenant.
/// Lit le TenantId depuis HttpContext.Items (placé par TenantMiddleware).
///
/// Injecté dans tous les services qui ont besoin du tenant courant.
/// </summary>
public sealed class CurrentTenantService : ICurrentTenant
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var items = _httpContextAccessor.HttpContext?.Items;
            if (items is not null &&
                items.TryGetValue("TenantId", out var value) &&
                value is Guid tenantId)
                return tenantId;

            return Guid.Empty;
        }
    }

    public string TenantName => string.Empty; // Enrichi plus tard
    public string? Plan => null;
    public bool IsResolved => TenantId != Guid.Empty;
}
