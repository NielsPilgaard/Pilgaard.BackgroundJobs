using Cronos;

namespace Pilgaard.CronJobs.Examples.OpenTelemetry;

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
        _logger.LogInformation("{jobName} executed at {now:G}", nameof(CronJob), DateTime.Now);

        const string endpoint = "https://localhost:7243/weatherforecast";

        await _client.GetAsync(endpoint, cancellationToken);
    }

    /// <summary>
    /// Executes once every second
    /// </summary>
    public CronExpression CronSchedule => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}
