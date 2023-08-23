using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.WorkerService;

public class RecurringJobEvery1Minute : IRecurringJob
{
    private readonly ILogger<RecurringJobEvery1Minute> _logger;
    public RecurringJobEvery1Minute(ILogger<RecurringJobEvery1Minute> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(RecurringJobEvery1Minute), DateTime.Now);

        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromMinutes(1);
}
