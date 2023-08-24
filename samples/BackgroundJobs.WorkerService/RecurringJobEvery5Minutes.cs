using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.WorkerService;

public class RecurringJobEvery5Minutes : IRecurringJob
{
    private readonly ILogger<RecurringJobEvery5Minutes> _logger;
    public RecurringJobEvery5Minutes(ILogger<RecurringJobEvery5Minutes> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(RecurringJobEvery5Minutes), DateTime.Now);

        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromMinutes(5);
}
