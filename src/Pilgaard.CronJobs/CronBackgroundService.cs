using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgaard.CronJobs.Configuration;

namespace Pilgaard.CronJobs;

/// <summary>
/// This class is responsible for running a <see cref="ICronJob"/>,
/// by hosting it as a <see cref="BackgroundService"/>,
/// <para>
/// The <see cref="CronBackgroundService"/> runs <see cref="ICronJob.ExecuteAsync"/>
/// whenever <see cref="ICronJob.CronSchedule"/> triggers.
/// </para>
/// </summary>
/// <remarks>
/// See also: <seealso cref="BackgroundService" />
/// </remarks>
public class CronBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICronJob _cronJob;
    private readonly ILogger<CronBackgroundService> _logger;
    private readonly CronJobOptions _options;
    private readonly CronExpression _cronSchedule;
    private readonly string _cronJobName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CronBackgroundService"/> class.
    /// </summary>
    /// <param name="cronJob">The cron job.</param>
    /// <param name="serviceScopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    public CronBackgroundService(
        ICronJob cronJob,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CronBackgroundService> logger,
        CronJobOptions options)
    {
        _options = options;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        // Lookup CronJob to get it's schedule without compromising its lifecycle
        // If lifetime is set to Singleton, the CronJob remains un-disposed.
        using var scope = _serviceScopeFactory.CreateScope();
        _cronJob = (ICronJob)scope.ServiceProvider.GetService(cronJob.GetType());
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
            _logger.LogDebug("The next time {cronJobName} will execute is {nextTaskOccurrence}", _cronJobName, nextTaskOccurrence);

            await PerformTaskOnNextOccurrence(nextTaskOccurrence, stoppingToken);

            nextTaskOccurrence = GetNextOccurrence();
        }
    }

    /// <summary>
    /// Performs the task on next occurrence.
    /// </summary>
    /// <param name="nextTaskExecution">The next task execution.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    private async Task PerformTaskOnNextOccurrence(
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

    /// <summary>
    ///     Gets the <see cref="DateTime"/> of the next time
    ///     <see cref="ICronJob.ExecuteAsync"/> should trigger.
    /// </summary>
    /// <returns>
    ///     The <see cref="DateTime"/> of the next time
    ///     <see cref="ICronJob.ExecuteAsync"/> should trigger.
    /// </returns>
    private DateTime? GetNextOccurrence()
    {
        return _cronSchedule.GetNextOccurrence(DateTime.UtcNow, _options.TimeZoneInfo);
    }

    /// <summary>
    ///     Gets the scoped <see cref="ICronJob"/> and executes it.
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    private async Task GetScopedJobAndExecute(CancellationToken stoppingToken)
    {
        _logger.LogDebug(
            "Fetching a {serviceLifetime} instance of {cronJobName} from the ServiceProvider.",
            _options.ServiceLifetime, _cronJobName);

        using var scope = _serviceScopeFactory.CreateScope();

        var cronJob = (ICronJob)scope.ServiceProvider.GetService(_cronJob.GetType());

        await cronJob.ExecuteAsync(stoppingToken);

        _logger.LogDebug("Successfully executed the CronJob {cronJobName}", _cronJobName);
    }

    /// <summary>
    ///     Checks whether the <paramref name="nextTaskExecution"/> is before
    ///     <see cref="DateTime.UtcNow"/>.
    /// </summary>
    /// <param name="nextTaskExecution">The next task execution.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="nextTaskExecution"/>
    ///     is before <see cref="DateTime.UtcNow"/>, otherwise <c>false</c>
    /// </returns>
    private static bool NextOccurrenceIsInThePast(DateTime? nextTaskExecution)
    {
        return DateTime.UtcNow > nextTaskExecution.GetValueOrDefault();
    }

    /// <summary>
    ///     Gets the <see cref="TimeSpan"/> until the next
    ///     <see cref="ICronJob.ExecuteAsync"/> should be triggered.
    /// </summary>
    /// <param name="nextTaskExecutionTime">The next task execution.</param>
    /// <returns>
    ///     The <see cref="TimeSpan"/> until the next
    ///     <see cref="ICronJob.ExecuteAsync"/> should be triggered.
    /// </returns>
    private static TimeSpan TimeUntilNextOccurrence(DateTime? nextTaskExecutionTime)
    {
        return nextTaskExecutionTime.GetValueOrDefault() - DateTime.UtcNow;
    }
}