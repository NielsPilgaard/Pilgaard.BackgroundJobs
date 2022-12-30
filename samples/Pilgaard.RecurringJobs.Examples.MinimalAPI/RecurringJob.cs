namespace Pilgaard.RecurringJobs.Examples.MinimalAPI;

public class RecurringJob : IRecurringJob
{
    private readonly ILogger<RecurringJob> _logger;
    private static readonly HttpClient _client = new();
    private int _number = 0;

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

        _number++;

        _logger.LogInformation("Number value: {number}", _number);
    }

    public TimeSpan Interval
    {
        get
        {
            _logger.LogInformation("");
            return TimeSpan.MaxValue;
        }
    }

    public ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
    public bool RunOnStartup => true;
}
