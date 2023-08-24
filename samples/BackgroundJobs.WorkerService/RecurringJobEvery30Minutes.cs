using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.WorkerService;

public class RecurringJobEvery30Minutes : IRecurringJob
{
    private readonly ILogger<RecurringJobEvery30Minutes> _logger;
    public RecurringJobEvery30Minutes(ILogger<RecurringJobEvery30Minutes> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(RecurringJobEvery30Minutes), DateTime.Now);

        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromMinutes(30);
}
