![Pilgaard BackgroundJobs Banner](https://user-images.githubusercontent.com/21295394/212175105-80087d36-42e3-436e-afbe-28c56173be60.png)
<div style="text-align: center">

[![CI](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/backgroundjobs_ci.yml/badge.svg)](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/backgroundjobs_ci.yml)
[![Downloads](https://img.shields.io/nuget/dt/pilgaard.backgroundjobs.svg)](https://www.nuget.org/packages/Pilgaard.BackgroundJobs)
[![Version](https://img.shields.io/nuget/vpre/pilgaard.backgroundjobs.svg)](https://www.nuget.org/packages/Pilgaard.BackgroundJobs)

</div>
A dotnet library for running background jobs in a scalable and performant manner.

## Features
- Implement background jobs through interfaces
- Centralized host to manage and run jobs, keeping memory and thread usage low.
- Dependency Injection support
- Read and update job schedules at runtime through `IConfiguration` or `IOptionsMonitor`
- Monitoring jobs using logs and metrics, both compatible with OpenTelemetry

## Scheduling Methods
- Cron expressions using `ICronJob`
- Recurringly at a set interval using `IRecurringJob`
- Once at an absolute time using `IOneTimeJob`

## Use Cases
- Sending emails 
- Processing data
- Enforcing data retention


# Getting Started
Make BackgroundJobs by implementing one of these interfaces:

```csharp
public class CronJob : ICronJob
{
    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Time to backup your databases!");

        return Task.CompletedTask;
    }
    public CronExpression CronExpression => CronExpression.Parse("0 3 * * *");
}
```
```csharp
public class RecurringJob : IRecurringJob
{
    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("This is your hourly reminder to stay hydrated.");

        return Task.CompletedTask;
    }
    public TimeSpan Interval => TimeSpan.FromHours(1);
}
```
```csharp
public class OneTimeJob : IOneTimeJob
{
    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Happy New Year!");

        return Task.CompletedTask;
    }
    public DateTime ScheduledTimeUtc => new(year: 2023, month: 12, day: 31, hour: 23, minute: 59, second: 59);
}
```


# Registration

Call `AddBackgroundJobs()` on an `IServiceCollection`, and then add jobs:

```csharp
builder.Services.AddBackgroundJobs()
    .AddJob<CronJob>()
    .AddJob<RecurringJob>()
    .AddJob<OneTimeJob>();
```

You can also register jobs in-line for simple use cases:

```csharp
builder.Services.AddBackgroundJobs()
    .AddJob(
        name: "basic-cronjob",
        job: () => {},
        cronExpression: CronExpression.Parse("* * * * *"))
    .AddJob(
        name: "basic-recurringjob",
        job: () => {},
        interval: TimeSpan.FromSeconds(3))
    .AddJob(
        name: "basic-onetimejob",
        job: () => {},
        scheduledTimeUtc: DateTime.UtcNow.AddHours(1))
    .AddAsyncJob(
        name: "async-cronjob",
        job: cancellationToken => Task.CompletedTask,
        cronExpression: CronExpression.Parse("* * * * *"))
    .AddAsyncJob(
        name: "async-recurringjob",
        job: cancellationToken => Task.CompletedTask,
        interval: TimeSpan.FromSeconds(3))
    .AddAsyncJob(
        name: "async-onetimejob",
        job: cancellationToken => Task.CompletedTask,
        scheduledTimeUtc: DateTime.UtcNow.AddHours(1));
```


# Samples

| Sample 🔗 | Tags |
| -- | -- |
| [BackgroundJobs.Configuration](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/samples/BackgroundJobs.Configuration) | ASP.NET, Reloading, Configuration
| [BackgroundJobs.MinimalAPI](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/samples/BackgroundJobs.MinimalAPI) | ASP.NET, MinimalAPI
| [BackgroundJobs.OpenTelemetry](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/samples/BackgroundJobs.OpenTelemetry) | ASP.NET, Open Telemetry, Metrics, Logs
| [BackgroundJobs.WorkerService](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/samples/BackgroundJobs.WorkerService) | Console, Worker Service

---

# Open Telemetry Compatibility

Each project exposes histogram metrics, which allow monitoring the duration and count of jobs.

The meter names match the project names.

The [Open Telemetry Sample](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/samples/BackgroundJobs.OpenTelemetry) shows how to collect CronJob metrics using the Prometheus Open Telemetry exporter.

---

## Roadmap

- ~~Replace Assembly Scanning with registration similar to that of HealthChecks~~
- A separate UI project to help visualize when jobs trigger
- More samples
  - Using Blazor Server
  - ~~Using a Worker Service~~
  - Using IConfiguration to reload job schedule
  - Using OneTimeJobs to control feature flags
  - Using RecurringJobs to manage data retention

---

## Thanks to

The developers of [Cronos](https://github.com/HangfireIO/Cronos) for their excellent Cron expression library.
