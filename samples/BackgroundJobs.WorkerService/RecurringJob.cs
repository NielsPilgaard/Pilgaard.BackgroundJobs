using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.WorkerService;

public class RecurringJob : IRecurringJob
{
    private readonly ILogger<RecurringJob> _logger;
    public RecurringJob(ILogger<RecurringJob> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(RecurringJob), DateTime.Now);

        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromMinutes(10);
}
