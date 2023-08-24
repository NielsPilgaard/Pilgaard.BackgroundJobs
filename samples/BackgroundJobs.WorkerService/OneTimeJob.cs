using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.WorkerService;

public class OneTimeJob : IOneTimeJob
{
    private readonly ILogger<OneTimeJob> _logger;
    private static readonly DateTime _utcNowAtStartup = DateTime.UtcNow;
    public OneTimeJob(ILogger<OneTimeJob> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(OneTimeJob), DateTime.Now);

        return Task.CompletedTask;
    }

    public DateTime ScheduledTimeUtc => _utcNowAtStartup.AddMinutes(1);
}
