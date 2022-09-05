using Cronos;

namespace Pilgaard.CronJobs.Examples.MinimalAPI;

public class ApiCallerCronJob : ICronJob
{
    private readonly ILogger<ApiCallerCronJob> _logger;
    private static readonly HttpClient Client = new();

    public ApiCallerCronJob(ILogger<ApiCallerCronJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing CronJob {nameofApiCallerCronJob} at {timeNow}",
            nameof(ApiCallerCronJob),
            DateTime.UtcNow.ToString("T"));

        const string endpoint = "https://localhost:7021/weatherforecast";

        await Client.GetAsync(endpoint, cancellationToken);
    }

    /// <summary>
    /// Executes once every second
    /// </summary>
    public CronExpression CronSchedule => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
}