using Pilgaard.BackgroundJobs;
using Pilgaard.RecurringJobs.Examples.WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddBackgroundJobs()
            .AddJob<RecurringJob>(nameof(RecurringJob));
    })
    .Build();

host.Run();
