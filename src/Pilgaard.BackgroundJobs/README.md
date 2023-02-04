# Pilgaard.BackgroundJobs

[![CI](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/backgroundjobs_ci.yml/badge.svg)](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/backgroundjobs_ci.yml)
[![Downloads](https://img.shields.io/nuget/dt/pilgaard.backgroundjobs.svg)](https://www.nuget.org/packages/Pilgaard.BackgroundJobs)
[![Version](https://img.shields.io/nuget/vpre/pilgaard.backgroundjobs.svg)](https://www.nuget.org/packages/Pilgaard.BackgroundJobs)

Easily create jobs that run in the background, with multiple different scheduling methods:

- [Cron Expressions](https://crontab.guru/)
- Recurringly at a set interval
- Absolute time

# Installing

With NuGet:

    Install-Package Pilgaard.BackgroundJobs

With the dotnet CLI:

    dotnet add package Pilgaard.BackgroundJobs

Or through Package Manager Console.

# Usage

Make BackgroundJobs by implementing one of these interfaces:
- `ICronJob`
- `IRecurringJob` 
- `IOneTimeJob`

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
    .AddJob<SampleCronJob>()
    .AddJob<SampleRecurringJob>()
    .AddJob<SampleScheduledJob>();
```

You can also register jobs in-line for simple use-cases:

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

## Thanks to

The developers of [Cronos](https://github.com/HangfireIO/Cronos) for their excellent Cron expression library.
