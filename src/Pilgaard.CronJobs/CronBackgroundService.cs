using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgaard.CronJobs.Extensions;

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
    private readonly string _jobName;

    private static readonly Meter _meter = new(
        name: typeof(CronBackgroundService).Assembly.GetName().Name!,
        version: typeof(CronBackgroundService).Assembly.GetName().Version?.ToString());

    private static readonly Histogram<double> _histogram =
        _meter.CreateHistogram<double>(
            name: $"{nameof(ICronJob)}.{nameof(ExecuteAsync)}".ToLower(),
            unit: "milliseconds",
            description: $"Histogram over duration and count of {nameof(ICronJob)}.{nameof(ICronJob.ExecuteAsync)}.");

    /// <summary>
    /// Initializes a new instance of the <see cref="CronBackgroundService"/> class.
    /// </summary>
    /// <param name="cronJob">The cronJob.</param>
    /// <param name="serviceScopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger.</param>
    public CronBackgroundService(
        ICronJob cronJob,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CronBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _cronJob = cronJob;
        _jobName = _cronJob.GetType().Name;

        _logger.LogInformation("Started {className} with Job {jobName}",
            nameof(CronBackgroundService), _jobName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var nextTaskOccurrence = GetNextOccurrence();

        while (nextTaskOccurrence is not null &&
               stoppingToken.IsCancellationRequested is false)
        {
            _logger.LogDebug("The next time {jobName} will execute is {nextTaskOccurrence}", _jobName, nextTaskOccurrence);

            await PerformTaskOnNextOccurrenceAsync(nextTaskOccurrence, stoppingToken).ConfigureAwait(false);

            nextTaskOccurrence = GetNextOccurrence();
        }
    }

    /// <summary>
    /// Performs the task on next occurrence.
    /// </summary>
    /// <param name="nextTaskExecution">The next task execution.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    private async Task PerformTaskOnNextOccurrenceAsync(
        DateTime? nextTaskExecution,
        CancellationToken stoppingToken)
    {
        // If UtcNow is higher than the time to execute next, we've already passed it
        if (NextOccurrenceIsInThePast(nextTaskExecution))
        {
            return;
        }

        var delay = TimeUntilNextOccurrence(nextTaskExecution);

        await Task.Delay(delay, stoppingToken).ConfigureAwait(false);

        // Measure duration of ExecuteAsync
        using var timer = _histogram.NewTimer(tags:
            new[]{
                new KeyValuePair<string, object?>("job_name", _jobName)
            });

        // If ServiceLifetime is Transient or Scoped, we need to re-fetch the
        // CronJob from the ServiceProvider on every execution.
        if (_cronJob.ServiceLifetime is not ServiceLifetime.Singleton)
        {
            _logger.LogDebug("Fetching a {serviceLifetime} instance of {jobName} from the ServiceProvider.",
                _cronJob.ServiceLifetime,
                _jobName);

            using var scope = _serviceScopeFactory.CreateScope();

            var cronJob = (ICronJob)scope.ServiceProvider.GetRequiredService(_cronJob.GetType());

            await cronJob.ExecuteAsync(stoppingToken).ConfigureAwait(false);

            _logger.LogDebug("Successfully executed the Job {jobName}", _jobName);

            return;
        }

        await _cronJob.ExecuteAsync(stoppingToken).ConfigureAwait(false);

        _logger.LogDebug("Successfully executed the Job {jobName}", _jobName);
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
        => _cronJob.CronSchedule.GetNextOccurrence(DateTime.UtcNow, _cronJob.TimeZoneInfo);

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
        => DateTime.UtcNow > nextTaskExecution.GetValueOrDefault();

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
        => nextTaskExecutionTime.GetValueOrDefault() - DateTime.UtcNow;
}
