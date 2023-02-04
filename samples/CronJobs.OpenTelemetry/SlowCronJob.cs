using Cronos;

namespace Pilgaard.CronJobs.Examples.OpenTelemetry;

public class SlowCronJob : ICronJob
{
    private readonly ILogger<SlowCronJob> _logger;

    public SlowCronJob(ILogger<SlowCronJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(SlowCronJob), DateTime.Now);

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }

    /// <summary>
    /// Executes once every second
    /// </summary>
    public CronExpression CronSchedule => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}
