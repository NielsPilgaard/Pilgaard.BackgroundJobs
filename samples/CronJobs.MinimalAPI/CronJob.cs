using Cronos;
using Pilgaard.CronJobs;

namespace CronJobs.MinimalAPI;

public class CronJob : ICronJob
{
    private readonly ILogger<CronJob> _logger;
    private static readonly HttpClient _client = new();

    public CronJob(ILogger<CronJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing CronJob {nameofApiCallerCronJob} at {timeNow}",
            nameof(CronJob),
            DateTime.UtcNow.ToString("T"));

        const string endpoint = "https://localhost:7021/weatherforecast";

        await _client.GetAsync(endpoint, cancellationToken);
    }

    /// <summary>
    /// Executes once every second
    /// </summary>
    public CronExpression CronSchedule => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}
