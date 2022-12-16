using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Pilgaard.CronJobs.Examples.OpenTelemetry;
using Pilgaard.CronJobs.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add CronJobs to the container.
builder.Services.AddCronJobs(typeof(Program));

// Configure OpenTelemetry settings
builder.AddOpenTelemetry();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", WeatherForecastEndpoint.Get);

app.Run();

namespace Pilgaard.CronJobs.Examples.OpenTelemetry
{
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
                    .AddMeter(typeof(CronBackgroundService).Namespace);
            });

            return builder;
        }
    }
}
