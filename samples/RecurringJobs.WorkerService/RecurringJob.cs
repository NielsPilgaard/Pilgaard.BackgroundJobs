namespace Pilgaard.RecurringJobs.Examples.WorkerService;

public class RecurringJob : IRecurringJob
{
    private readonly ILogger<RecurringJob> _logger;
    public RecurringJob(ILogger<RecurringJob> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(RecurringJob), DateTime.Now);

        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromMinutes(10);
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
    public TimeSpan InitialDelay => TimeSpan.Zero;
}
