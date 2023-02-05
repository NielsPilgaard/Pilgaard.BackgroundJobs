using BackgroundJobs.OpenTelemetry;
using Pilgaard.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

// Add CronJobs to the container.
builder.Services.AddBackgroundJobs()
    .AddJob<CronJob>(nameof(CronJob))
    .AddJob<SlowCronJob>(nameof(SlowCronJob));

// Configure OpenTelemetry settings
builder.AddOpenTelemetry();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", WeatherForecastEndpoint.Get);

app.Run();
