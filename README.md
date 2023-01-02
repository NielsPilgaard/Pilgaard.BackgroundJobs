# Pilgaard.BackgroundJobs

Easily create jobs that run in the background, with multiple different scheduling methods.

| Package 🔗           | Version & Downloads                                                                                                                                                       | Description |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----- |
| [CronJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.CronJobs) | [![Version](https://img.shields.io/nuget/vpre/pilgaard.cronjobs.svg)](https://www.nuget.org/packages/Pilgaard.CronJobs)[![Nuget](https://img.shields.io/nuget/dt/Pilgaard.CronJobs)](https://www.nuget.org/packages/Pilgaard.CronJobs) | Background Jobs that trigger based on [Cron Expressions](https://crontab.guru/)
| [RecurringJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.RecurringJobs) | [![Version](https://img.shields.io/nuget/vpre/pilgaard.recurringjobs.svg)](https://www.nuget.org/packages/Pilgaard.RecurringJobs)[![Nuget](https://img.shields.io/nuget/dt/Pilgaard.RecurringJobs)](https://www.nuget.org/packages/Pilgaard.RecurringJobs) | Background Jobs that trigger based on intervals.
| [ScheduledJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.ScheduledJobs) | [![Version](https://img.shields.io/nuget/vpre/pilgaard.scheduledjobs.svg)](https://www.nuget.org/packages/Pilgaard.ScheduledJobs)[![Nuget](https://img.shields.io/nuget/dt/Pilgaard.ScheduledJobs)](https://www.nuget.org/packages/Pilgaard.ScheduledJobs) | Background Jobs that trigger once at a specific date and time.

---

# Installing

With NuGet:

    Install-Package Pilgaard.CronJobs
    Install-Package Pilgaard.RecurringJobs
    Install-Package Pilgaard.ScheduledJobs

With the dotnet CLI:

    dotnet add package Pilgaard.CronJobs
    dotnet add package Pilgaard.RecurringJobs
    dotnet add package Pilgaard.ScheduledJobs

Or through Package Manager Console.


# Getting Started

Each nuget has instructions on how to get started: 

[Pilgaard.CronJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.CronJobs)

[Pilgaard.RecurringJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.RecurringJobs)

[Pilgaard.ScheduledJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.ScheduledJobs)


---


## Roadmap

- More samples
  - Using Blazor
  - Using a Worker Service
  - Using IConfiguration in RecurringJobs
  - Using ScheduledJobs to control feature flags
  - Using RecurringJobs to manage data retention
  - Registering Jobs from an external assembly

---

## Thanks to

The developers of [Cronos](https://github.com/HangfireIO/Cronos) for their excellent Cron expression library.
