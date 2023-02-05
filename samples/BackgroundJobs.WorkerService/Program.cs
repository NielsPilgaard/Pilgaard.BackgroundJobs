using BackgroundJobs.WorkerService;
using Pilgaard.BackgroundJobs;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddBackgroundJobs()
            .AddJob<RecurringJob>(nameof(RecurringJob));
    })
    .Build();

host.Run();
