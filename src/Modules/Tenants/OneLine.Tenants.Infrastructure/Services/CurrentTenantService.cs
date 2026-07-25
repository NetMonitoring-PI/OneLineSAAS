using Microsoft.AspNetCore.Http;
using OneLine.Shared.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Services;

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

    public string TenantName => string.Empty;
    public string? Plan => null;
    public bool IsResolved => TenantId != Guid.Empty;
}
