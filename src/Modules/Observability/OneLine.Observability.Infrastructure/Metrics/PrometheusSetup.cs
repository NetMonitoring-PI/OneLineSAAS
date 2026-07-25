using Prometheus;

namespace OneLine.Observability.Infrastructure.Metrics;

public static class PrometheusSetup
{
    public static readonly Counter HttpRequestsTotal = Prometheus.Metrics
        .CreateCounter(
            "oneline_http_requests_total",
            "Nombre total de requetes HTTP",
            new CounterConfiguration
            {
                LabelNames = ["method", "path", "status"]
            });

    public static readonly Histogram HttpRequestDuration = Prometheus.Metrics
        .CreateHistogram(
            "oneline_http_request_duration_seconds",
            "Duree des requetes HTTP en secondes",
            new HistogramConfiguration
            {
                LabelNames = ["method", "path"],
                Buckets = [0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5]
            });

    public static readonly Gauge ActiveTenants = Prometheus.Metrics
        .CreateGauge(
            "oneline_active_tenants",
            "Nombre de tenants actifs dans le systeme");
}
