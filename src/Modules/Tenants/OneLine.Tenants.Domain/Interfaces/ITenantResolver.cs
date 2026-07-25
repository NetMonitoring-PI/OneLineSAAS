using Microsoft.AspNetCore.Http;

namespace OneLine.Tenants.Domain.Interfaces;

public interface ITenantResolver
{
    int Priority { get; }
    Task<Guid?> ResolveAsync(HttpContext context);
}
