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
        _logger.LogInformation("Executing CronJob {nameofApiCallerCronJob} at {timeNow}",
            nameof(SlowCronJob),
            DateTime.UtcNow.ToString("T"));

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }

    /// <summary>
    /// Executes once every second
    /// </summary>
    public CronExpression CronSchedule => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}
