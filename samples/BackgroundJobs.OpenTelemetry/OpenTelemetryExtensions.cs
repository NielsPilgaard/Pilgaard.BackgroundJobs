using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace BackgroundJobs.OpenTelemetry;

internal static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: builder.Environment.ApplicationName,
                serviceNamespace: typeof(Program).Namespace,
                serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString());

        builder.Services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddPrometheusExporter()
                    .AddConsoleExporter()
                    .SetResourceBuilder(resourceBuilder)
                    .AddMeter("Pilgaard.BackgroundJobs");
            })
            .StartWithHost();

        return builder;
    }
}
