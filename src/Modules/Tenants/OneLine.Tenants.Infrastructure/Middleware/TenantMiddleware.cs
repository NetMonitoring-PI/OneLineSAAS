using Microsoft.AspNetCore.Http;
using OneLine.Tenants.Domain.Interfaces;

namespace OneLine.Tenants.Infrastructure.Middleware;

public sealed class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IEnumerable<ITenantResolver> resolvers)
    {
        foreach (var resolver in resolvers.OrderBy(r => r.Priority))
        {
            var tenantId = await resolver.ResolveAsync(context);
            if (tenantId.HasValue)
            {
                context.Items["TenantId"] = tenantId.Value;
                break;
            }
        }

        await _next(context);
    }
}
