# Pilgaard.ScheduledJobs

[![CI](https://github.com/NillerMedDild/Pilgaard.BackgroundJobs/workflows/scheduledjobs_ci/badge.svg)](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/scheduledjobs_ci.yml)
[![Downloads](https://img.shields.io/nuget/dt/pilgaard.ScheduledJobs.svg)](https://www.nuget.org/packages/Pilgaard.ScheduledJobs)
[![Version](https://img.shields.io/nuget/vpre/pilgaard.ScheduledJobs.svg)](https://www.nuget.org/packages/Pilgaard.ScheduledJobs)

Easily create jobs that run at a specific time and date.

# Installing

With NuGet:

    Install-Package Pilgaard.ScheduledJobs

With the dotnet CLI:

    dotnet add package Pilgaard.ScheduledJobs

Or through Package Manager Console.

# Usage

Make ScheduledJobs by implementing `IScheduledJob`:

```csharp
public class ScheduledJob : IScheduledJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Do work

        return Task.CompletedTask;
    }

    public DateTime ScheduledTimeUtc => DateTime.UtcNow.AddSeconds(5);
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}
```

# Registration

Register all ScheduledJobs with an `IServiceCollection` instance:

```csharp
services.AddScheduledJobs(typeof(Program));
```

This will scan the assembly of `Program` for all classes that implement `IScheduledJob`, and add them to the container.

Each `IScheduledJob` found is then hosted in a [ScheduledJobBackgroundService](https://github.com/NielsPilgaard/Pilgaard.Jobs/blob/master/src/Pilgaard.ScheduledJobs/ScheduledJobBackgroundService.cs).
