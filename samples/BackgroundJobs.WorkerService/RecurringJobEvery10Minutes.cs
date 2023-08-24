using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.WorkerService;

public class RecurringJobEvery10Minutes : IRecurringJob
{
    private readonly ILogger<RecurringJobEvery10Minutes> _logger;
    public RecurringJobEvery10Minutes(ILogger<RecurringJobEvery10Minutes> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(RecurringJobEvery10Minutes), DateTime.Now);

        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromMinutes(10);
}
