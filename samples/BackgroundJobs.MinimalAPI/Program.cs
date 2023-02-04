using Cronos;
using Pilgaard.BackgroundJobs;
using Pilgaard.BackgroundJobs.Examples.MinimalAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBackgroundJobs()
    .AddJob<CronJob>(name: nameof(CronJob))
    .AddJob(
        name: "basic-cronjob",
        job: () => Console.WriteLine("I happened!"),
        cronExpression: CronExpression.Parse("* * * * *"))
    .AddJob(
        name: "basic-recurringjob",
        job: () => Console.WriteLine("I happened!"),
        interval: TimeSpan.FromSeconds(3))
    .AddJob(
        name: "basic-onetimejob",
        job: () => Console.WriteLine($"Triggered at {DateTime.Now}"),
        scheduledTimeUtc: DateTime.UtcNow.AddHours(1))
    .AddAsyncJob(
        name: "async-cronjob",
        job: cancellationToken => Task.CompletedTask,
        cronExpression: CronExpression.Parse("* * * * *"))
    .AddAsyncJob(
        name: "async-recurringjob",
        job: async (cancellationToken) => await new HttpClient().GetStringAsync("https://localhost:7149/weatherforecast", cancellationToken: cancellationToken),
        interval: TimeSpan.FromSeconds(3))
    .AddAsyncJob(
        name: "async-onetimejob",
        job: cancellationToken => Task.CompletedTask,
        scheduledTimeUtc: DateTime.UtcNow.AddHours(1));
;

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", WeatherForecastEndpoint.Get);

app.Run();
