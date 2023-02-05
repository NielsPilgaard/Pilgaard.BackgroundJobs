using Cronos;
using Pilgaard.BackgroundJobs;

namespace BackgroundJobs.OpenTelemetry;

public class SlowCronJob : ICronJob
{
    private readonly ILogger<SlowCronJob> _logger;

    public SlowCronJob(ILogger<SlowCronJob> logger)
    {
        _logger = logger;
    }

    public async Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(SlowCronJob), DateTime.Now);

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }

    /// <summary>
    /// Executes once every second
    /// </summary>
    public CronExpression CronExpression => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
}
