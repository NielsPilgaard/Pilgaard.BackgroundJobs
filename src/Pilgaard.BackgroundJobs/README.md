# Pilgaard.BackgroundJobs

[![CI](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/backgroundjobs_ci.yml/badge.svg)](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/actions/workflows/backgroundjobs_ci.yml)
[![Downloads](https://img.shields.io/nuget/dt/pilgaard.backgroundjobs.svg)](https://www.nuget.org/packages/Pilgaard.BackgroundJobs)
[![Version](https://img.shields.io/nuget/vpre/pilgaard.backgroundjobs.svg)](https://www.nuget.org/packages/Pilgaard.BackgroundJobs)

Easily create jobs that run in the background, with multiple different scheduling methods.

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
- `IScheduledJob`

```csharp

```


# Registration

Calling `AddBackgroundJobs()` on an `IServiceCollection`, and then add jobs:

```csharp
builder.Services.AddBackgroundJobs()
    .AddJob<SampleCronJob>()
    .AddJob<SampleRecurringJob>()
    .AddJob<SampleScheduledJob>();
```


## Thanks to

The developers of [Cronos](https://github.com/HangfireIO/Cronos) for their excellent Cron expression library.
