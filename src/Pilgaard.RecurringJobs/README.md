# Pilgaard.RecurringJobs

[![CI](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/recurringjobs_ci.yml/badge.svg)](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/recurringjobs_ci.yml)
[![Downloads](https://img.shields.io/nuget/dt/Pilgaard.RecurringJobs.svg)](https://www.nuget.org/packages/Pilgaard.RecurringJobs)
[![Version](https://img.shields.io/nuget/vpre/Pilgaard.RecurringJobs.svg)](https://www.nuget.org/packages/Pilgaard.RecurringJobs)

Easily create recurring jobs that run at intervals.

# Installing

With NuGet:

    Install-Package Pilgaard.RecurringJobs

With the dotnet CLI:

    dotnet add package Pilgaard.RecurringJobs

Or through Package Manager Console.

# Usage

Make RecurringJobs by implementing `IRecurringJob`:

```csharp
public class RecurringJob : IRecurringJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Do work

        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromSeconds(15);
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
    public TimeSpan InitialDelay => TimeSpan.Zero;
}
```

# Registration

Register all RecurringJobs with an `IServiceCollection` instance:

```csharp
services.AddRecurringJobs(typeof(Program));
```

This will scan the assembly of `Program` for all classes that implement `IRecurringJob`, and add them to the container.

Each `IRecurringJob` found is then hosted in a [RecurringJobBackgroundService](https://github.com/NielsPilgaard/Pilgaard.Jobs/blob/master/src/Pilgaard.RecurringJobs/RecurringJobBackgroundService.cs).
