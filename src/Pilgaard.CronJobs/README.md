# Pilgaard.CronJobs

[![CI](https://github.com/NillerMedDild/Pilgaard.BackgroundJobs/workflows/cronjobs_ci/badge.svg)](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/cronjobs_ci.yml)
[![Downloads](https://img.shields.io/nuget/dt/pilgaard.cronjobs.svg)](https://www.nuget.org/packages/Pilgaard.CronJobs)
[![Version](https://img.shields.io/nuget/vpre/pilgaard.cronjobs.svg)](https://www.nuget.org/packages/Pilgaard.CronJobs)

Easily schedule jobs to run at specific times, based on Cron expressions.

# Installing

With NuGet:

    Install-Package Pilgaard.CronJobs

With the dotnet CLI:

    dotnet add package Pilgaard.CronJobs

Or through Package Manager Console.

# Usage

Make CronJobs by implementing `ICronJob`:

```csharp
public class CronJob : ICronJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Execute job

        return Task.CompletedTask;
    }

    // This will execute once every minute
    public CronExpression CronSchedule => CronExpression.Parse("* * * * *");
}
```


# Registration

Register all CronJobs with an `IServiceCollection` instance:

```csharp
services.AddCronJobs(typeof(Program));
```

This will scan the assembly of `Program` for all classes that implement `ICronJob`, and add them to the container.

Each `ICronJob` found is then hosted in a [CronBackgroundService](https://github.com/NielsPilgaard/Pilgaard.CronJobs/blob/master/src/Pilgaard.CronJobs/CronBackgroundService.cs).


## Thanks to

The developers of [Cronos](https://github.com/HangfireIO/Cronos) for their excellent Cron expression library.
