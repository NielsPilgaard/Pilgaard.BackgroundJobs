![Pilgaard BackgroundJobs Banner](https://user-images.githubusercontent.com/21295394/212175105-80087d36-42e3-436e-afbe-28c56173be60.png)

Multiple scheduling methods are supported:

- [CronExpressions](https://crontab.guru/)
- Recurringly at a set interval
- Absolute time

| Package 🔗           | Version & Downloads                                                                                                                                                       | Description |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----- |
| [CronJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.CronJobs) | [![Version](https://img.shields.io/nuget/vpre/pilgaard.cronjobs.svg)](https://www.nuget.org/packages/Pilgaard.CronJobs)[![Nuget](https://img.shields.io/nuget/dt/Pilgaard.CronJobs)](https://www.nuget.org/packages/Pilgaard.CronJobs) | Background Jobs that trigger based on [Cron Expressions](https://crontab.guru/)
| [RecurringJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.RecurringJobs) | [![Version](https://img.shields.io/nuget/vpre/pilgaard.recurringjobs.svg)](https://www.nuget.org/packages/Pilgaard.RecurringJobs)[![Nuget](https://img.shields.io/nuget/dt/Pilgaard.RecurringJobs)](https://www.nuget.org/packages/Pilgaard.RecurringJobs) | Background Jobs that trigger based on intervals.
| [ScheduledJobs](https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.ScheduledJobs) | [![Version](https://img.shields.io/nuget/vpre/pilgaard.scheduledjobs.svg)](https://www.nuget.org/packages/Pilgaard.ScheduledJobs)[![Nuget](https://img.shields.io/nuget/dt/Pilgaard.ScheduledJobs)](https://www.nuget.org/packages/Pilgaard.ScheduledJobs) | Background Jobs that trigger once at a specific date and time.

# Getting Started

<ul>
  <li>
    <a href="https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.CronJobs" target="_blank" >CronJobs</a>
  </li>
  <li>
    <a href="https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.RecurringJobs" target="_blank">RecurringJobs</a>
  </li>
  <li>
    <a href="https://github.com/NielsPilgaard/Pilgaard.BackgroundJobs/tree/master/src/Pilgaard.ScheduledJobs" target="_blank">ScheduledJobs</a>
  </li>
</ul>

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


---

# Open Telemetry Compatibility

Each project exposes histogram metrics, which allow monitoring the duration and count of `ExecuteAsync` invocations.

The meter names match the project names.

The [Open Telemetry Sample](https://github.com/NielsPilgaard/Pilgaard.CronJobs/tree/master/samples/Pilgaard.CronJobs.Examples.OpenTelemetry) shows how to collect CronJob metrics using the Prometheus Open Telemetry exporter.

---

## Roadmap

- Replace Assembly Scanning with registration similar to that of HealthChecks
- A separate UI project to help visualize when jobs trigger
- More samples
  - Using Blazor
  - ~~Using a Worker Service~~
  - Using IConfiguration in RecurringJobs
  - Using ScheduledJobs to control feature flags
  - Using RecurringJobs to manage data retention
  - Registering Jobs from an external assembly

---

## Thanks to

The developers of [Cronos](https://github.com/HangfireIO/Cronos) for their excellent Cron expression library.
