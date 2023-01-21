using Cronos;
using Pilgaard.BackgroundJobs.Examples.MinimalAPI;
using Pilgaard.BackgroundJobs.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBackgroundJobs()
    .AddJob(
        name: "basic-cronjob",
        job: () => Console.WriteLine("I happened!"),
        cronExpression: CronExpression.Parse("* * * * *", CronFormat.IncludeSeconds))
    .AddAsyncJob(
        name: "async-recurringjob",
        job: async (cancellationToken) => await new HttpClient().GetStringAsync("https://localhost:7149/weatherforecast", cancellationToken: cancellationToken),
        interval: TimeSpan.FromSeconds(3))
    .AddJob(
        name: "basic-onetimejob",
        job: () => Console.WriteLine($"Triggered at {DateTime.Now}"),
        scheduledTimeUtc: DateTime.UtcNow.AddHours(1))
    .AddJob<CronJob>(name: nameof(CronJob));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", WeatherForecastEndpoint.Get);

app.Run();
