using Cronos;
using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.WorkerService;

public class CronJob : ICronJob
{
    private readonly ILogger<CronJob> _logger;
    public CronJob(ILogger<CronJob> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(CronJob), DateTime.Now);

        return Task.CompletedTask;
    }

    public CronExpression CronExpression => CronExpression.Parse("* * * * *");
}
