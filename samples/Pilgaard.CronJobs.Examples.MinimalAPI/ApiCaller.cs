using Cronos;

namespace Pilgaard.CronJobs.Examples.MinimalAPI;

public class ApiCaller : ICronJob
{
    private readonly ILogger<ApiCaller> _logger;
    private readonly HttpClient _client = new();

    public ApiCaller(ILogger<ApiCaller> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing CronJob {implementation}.", nameof(ApiCaller));

        const string endpoint = "https://localhost:7021/weatherforecast";

        var response = await _client.GetStringAsync(endpoint, cancellationToken);

        _logger.LogInformation("Received response from endpoint {endpoint}: {response}",
            endpoint, response);
    }

    /// <summary>
    /// Every minute
    /// </summary>
    public CronExpression CronSchedule => CronExpression.Parse("* * * * *");
}