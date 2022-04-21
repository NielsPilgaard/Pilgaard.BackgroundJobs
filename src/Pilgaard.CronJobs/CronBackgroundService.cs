using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pilgaard.CronJobs;

public class CronBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICronJob _cronJob;
    private readonly ILogger<CronBackgroundService> _logger;
    private readonly CronJobOptions _options;
    private readonly CronExpression _cronSchedule;
    private readonly string _cronJobName;

    public CronBackgroundService(
        ICronJob cronJob,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CronBackgroundService> logger,
        Action<CronJobOptions>? configuration = null)
    {
        _options = new CronJobOptions();
        configuration?.Invoke(_options);

        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        // Lookup CronJob to get it's schedule without compromising its lifecycle
        // If lifetime is set to Singleton, the CronJob remains un-disposed.
        using var scope = _serviceScopeFactory.CreateScope();
        _cronJob = (ICronJob)scope.ServiceProvider.GetRequiredService(cronJob.GetType());
        _cronJobName = _cronJob.GetType().Name;
        _cronSchedule = _cronJob.CronSchedule;

        _logger.LogInformation("Started CronBackgroundService with CronJob {cronJobName}", _cronJobName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var nextTaskOccurrence = GetNextOccurrence();
        while (nextTaskOccurrence is not null &&
               stoppingToken.IsCancellationRequested is false)
        {
            _logger.LogDebug("The next time {cronJobName} will execute is {nextTaskOccurrence:}", _cronJobName, nextTaskOccurrence);

            await PerformTaskOnNextOccurrence(nextTaskOccurrence, stoppingToken);

            nextTaskOccurrence = GetNextOccurrence();
        }
    }

    public virtual async Task PerformTaskOnNextOccurrence(
        DateTime? nextTaskExecution,
        CancellationToken stoppingToken)
    {
        // If UtcNow is higher than the time to execute next, we've already passed it
        if (NextOccurrenceIsInThePast(nextTaskExecution))
        {
            return;
        }

        var delay = TimeUntilNextOccurrence(nextTaskExecution);

        await Task.Delay(delay, stoppingToken);

        // If ServiceLifetime is Transient or Scoped, we need to re-fetch the
        // CronJob from the ServiceProvider on every execution.
        if (_options.ServiceLifetime is not ServiceLifetime.Singleton)
        {
            await GetScopedJobAndExecute(stoppingToken);
            return;
        }

        await _cronJob.ExecuteAsync(stoppingToken);
    }

    private async Task GetScopedJobAndExecute(CancellationToken stoppingToken)
    {
        _logger.LogDebug(
            "Fetching a {serviceLifetime} instance of {cronJobName} from the ServiceProvider.",
            _options.ServiceLifetime, _cronJobName);

        using var scope = _serviceScopeFactory.CreateScope();

        var cronJob = (ICronJob)scope.ServiceProvider.GetRequiredService(_cronJob.GetType());

        await cronJob.ExecuteAsync(stoppingToken);

        _logger.LogDebug("Successfully executed the CronJob {cronJobName}", _cronJobName);
    }

    private static bool NextOccurrenceIsInThePast(DateTime? nextTaskExecution)
    {
        return DateTime.UtcNow > nextTaskExecution.GetValueOrDefault();
    }

    private static TimeSpan TimeUntilNextOccurrence(DateTime? nextTaskExecution)
    {
        return nextTaskExecution.GetValueOrDefault() - DateTime.UtcNow;
    }

    public virtual DateTime? GetNextOccurrence()
    {
        return _cronSchedule.GetNextOccurrence(DateTime.UtcNow, _options.TimeZoneInfo);
    }
}