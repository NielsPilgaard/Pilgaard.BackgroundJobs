# Pilgaard.CronJobs

![CI](https://github.com/NillerMedDild/Pilgaard.CronJobs/workflows/Release/badge.svg)
[![NuGet](https://img.shields.io/nuget/dt/pilgaard.cronjobs.svg)](https://www.nuget.org/packages/mediatr)
[![NuGet](https://img.shields.io/nuget/vpre/pilgaard.cronjobs.svg)](https://www.nuget.org/packages/mediatr)



Easily schedule jobs to run at specific times, based on Cron expressions.





## Installing

With NuGet:

    Install-Package Pilgaard.CronJobs

With the .NET CLI:

    dotnet add package Pilgaard.CronJobs

Or through Package Manager Console.





## Usage

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





### Registration

Register all CronJobs with an `IServiceCollection` instance:

```csharp
services.AddCronJobs(typeof(Program));
```



This will scan the assembly for all classes that implement `ICronJob`, and add them to the container.

Each `ICronJob` found is then hosted in a [CronBackgroundService](https://github.com/NielsPilgaard/Pilgaard.CronJobs/blob/master/src/Pilgaard.CronJobs/CronBackgroundService.cs).





### Configuration

The following options are available for customizing `ICronJob` behaviour:

```csharp
services.AddCronJobs(options =>
{
    options.ServiceLifetime = ServiceLifetime.Singleton;
    options.CronFormat = CronFormat.IncludeSeconds;
    options.TimeZoneInfo = TimeZoneInfo.Utc;
}, typeof(Program));
```



## Roadmap



- Summaries for all the things
- Actual tests
- More (realistic) samples
  - Using Blazor
  - Using a Worker Service
  - Registering `CronJobs` from an external assembly, such as a class library



---



## Thanks to

The developers of [Cronos](https://github.com/HangfireIO/Cronos) for their excellent Cron expression library.
