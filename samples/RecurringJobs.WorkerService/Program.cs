using Pilgaard.RecurringJobs.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddRecurringJobs(typeof(Program));
    })
    .Build();

host.Run();
