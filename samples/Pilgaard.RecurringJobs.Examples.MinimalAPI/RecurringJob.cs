namespace Pilgaard.RecurringJobs.Examples.MinimalAPI;

public class RecurringJob : IRecurringJob
{
    private readonly ILogger<RecurringJob> _logger;
    private static readonly HttpClient _client = new();

    public RecurringJob(ILogger<RecurringJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing RecurringJob {nameofApiCallerJob} at {timeNow}",
            nameof(RecurringJob),
            DateTime.UtcNow.ToString("T"));

        const string endpoint = "https://localhost:7021/weatherforecast";

        await _client.GetAsync(endpoint, cancellationToken);
    }

    public TimeSpan Interval => TimeSpan.FromSeconds(5);

    public ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
    public TimeSpan InitialDelay => TimeSpan.Zero;
}
