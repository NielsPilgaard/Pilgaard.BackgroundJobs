using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Pilgaard.CronJobs.Examples.OpenTelemetry;

internal static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: builder.Environment.ApplicationName,
                serviceNamespace: typeof(Program).Namespace,
                serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString());

        builder.Services.AddOpenTelemetryMetrics(metrics =>
        {
            metrics
                .AddPrometheusExporter()
                .AddConsoleExporter()
                .SetResourceBuilder(resourceBuilder)
                .AddMeter("Pilgaard.BackgroundJobs");
        });

        return builder;
    }
}
