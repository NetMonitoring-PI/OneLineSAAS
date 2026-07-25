using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace OneLine.Observability.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static IHostBuilder ConfigureSerilog(
        this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, config) =>
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/oneline-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7);
        });
    }
}
