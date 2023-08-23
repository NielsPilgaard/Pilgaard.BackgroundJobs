using BackgroundJobs.WorkerService;
using Pilgaard.BackgroundJobs;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddBackgroundJobs()
            .AddJob<RecurringJobEvery1Minute>()
            .AddJob<RecurringJobEvery5Minutes>()
            .AddJob<RecurringJobEvery10Minutes>()
            .AddJob<RecurringJobEvery30Minutes>()
            .AddJob<CronJob>()
            .AddJob<OneTimeJob>();
    })
    .Build().Run();
