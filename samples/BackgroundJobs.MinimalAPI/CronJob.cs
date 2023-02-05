using Cronos;
using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.MinimalAPI;

public class CronJob : ICronJob
{
    private readonly ILogger<CronJob> _logger;

    public CronJob(ILogger<CronJob> logger)
    {
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("I run once per minute");

        return Task.CompletedTask;
    }

    public CronExpression CronExpression => CronExpression.Parse("* * * * *");
}
