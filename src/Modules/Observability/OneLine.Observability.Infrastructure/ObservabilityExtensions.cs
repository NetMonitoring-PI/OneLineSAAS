using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneLine.Observability.Infrastructure.Middleware;
using Prometheus;

namespace OneLine.Observability.Infrastructure;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservabilityInfrastructure(
        this IServiceCollection services)
    {
        return services;
    }

    public static IApplicationBuilder UseObservability(
        this IApplicationBuilder app)
    {
        // Correlation ID sur chaque requete
        app.UseMiddleware<CorrelationIdMiddleware>();

        // Logging de chaque requete
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Endpoint Prometheus /metrics
        app.UseMetricServer();
        app.UseHttpMetrics();

        return app;
    }
}
