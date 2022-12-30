using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgaard.ScheduledJobs.Configuration;
using Pilgaard.ScheduledJobs.Extensions;

namespace Pilgaard.ScheduledJobs;

/// <summary>
/// This class is responsible for running a <see cref="IScheduledJob"/>,
/// by hosting it as a <see cref="BackgroundService"/>,
/// <para>
/// The <see cref="ScheduledJobBackgroundService"/> runs <see cref="IScheduledJob.ExecuteAsync"/>
/// whenever <see cref="IScheduledJob"/> triggers.
/// </para>
/// </summary>
/// <remarks>
/// See also: <seealso cref="BackgroundService" />
/// </remarks>
public class ScheduledJobBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IScheduledJob _job;
    private readonly ILogger<ScheduledJobBackgroundService> _logger;
    private readonly ScheduledJobOptions _options;
    private readonly string _jobName;

    private static readonly Meter _meter = new(
        name: typeof(ScheduledJobBackgroundService).Assembly.GetName().Name!,
        version: typeof(ScheduledJobBackgroundService).Assembly.GetName().Version?.ToString());

    private static readonly Histogram<double> _histogram =
        _meter.CreateHistogram<double>(
            name: $"{nameof(IScheduledJob)}.{nameof(ExecuteAsync)}".ToLower(),
            unit: "milliseconds",
            description: $"Histogram over duration and count of {nameof(IScheduledJob)}.{nameof(IScheduledJob.ExecuteAsync)}.");

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledJobBackgroundService"/> class.
    /// </summary>
    /// <param name="job">The job.</param>
    /// <param name="serviceScopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    public ScheduledJobBackgroundService(
        IScheduledJob job,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ScheduledJobBackgroundService> logger,
        ScheduledJobOptions options)
    {
        _options = options;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _job = job;
        _jobName = _job.GetType().Name;

        _logger.LogInformation("Started {className} with Job {jobName}", nameof(ScheduledJobBackgroundService), _jobName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scheduledTime = _job.ScheduledTimeUtc;

        if (ScheduledTimeIsInThePast(scheduledTime))
        {
            _logger.LogWarning("The current time of {datetimeUtcNow} is higher than the scheduled time of {scheduledTime}, no action will be performed.", DateTime.UtcNow, scheduledTime);
            return;
        }

        _logger.LogInformation("{jobName} will trigger {scheduledTime:F}", _jobName, scheduledTime);

        var delay = scheduledTime.Subtract(DateTime.UtcNow);

        _logger.LogDebug("Time until {jobName} triggers: {delay}", _jobName, delay);

        await Task.Delay(delay, stoppingToken);

        await InternalExecuteAsync(stoppingToken);
    }

    /// <summary>
    /// Performs the <see cref="IScheduledJob.ExecuteAsync"/> task.
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    private async Task InternalExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        // Measure duration of ExecuteAsync
        using var timer = _histogram.NewTimer(tags:
            new[]{
                new KeyValuePair<string, object?>("job_name", _jobName)
            });

        // If ServiceLifetime is Transient or Scoped, we need to re-fetch the
        // Job from the ServiceProvider on every execution.
        if (_options.ServiceLifetime is not ServiceLifetime.Singleton)
        {
            _logger.LogDebug(
                "Fetching a {serviceLifetime} instance of {jobName} from the IServiceScopeFactory.",
                _options.ServiceLifetime, _jobName);

            using var scope = _serviceScopeFactory.CreateScope();

            var job = (IScheduledJob)scope.ServiceProvider.GetRequiredService(_job.GetType());

            await job.ExecuteAsync(stoppingToken).ConfigureAwait(false);

            _logger.LogDebug("Successfully executed the Job {jobName}", _jobName);

            return;
        }

        await _job.ExecuteAsync(stoppingToken);
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
    private static bool ScheduledTimeIsInThePast(DateTime? nextTaskExecution)
        => DateTime.UtcNow > nextTaskExecution.GetValueOrDefault();

    /// <summary>
    ///     Gets the <see cref="TimeSpan"/> until the
    ///     <see cref="IScheduledJob.ExecuteAsync"/> should be triggered.
    /// </summary>
    /// <param name="scheduledTime">The scheduled time.</param>
    /// <returns>
    ///     The <see cref="TimeSpan"/> until the
    ///     <see cref="IScheduledJob.ExecuteAsync"/> should be triggered.
    /// </returns>
    private static TimeSpan TimeUntilNextOccurrence(DateTime scheduledTime)
        => scheduledTime.Subtract(DateTime.UtcNow);
}
