using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgaard.RecurringJobs.Extensions;

namespace Pilgaard.RecurringJobs;

/// <summary>
/// This class is responsible for running a <see cref="IRecurringJob"/>,
/// by hosting it as a <see cref="BackgroundService"/>,
/// <para>
/// The <see cref="RecurringJobBackgroundService"/> runs <see cref="IRecurringJob.ExecuteAsync"/>
/// whenever <see cref="IRecurringJob.Interval"/> time has elapsed.
/// </para>
/// </summary>
/// <remarks>
/// See also: <seealso cref="BackgroundService" />
/// </remarks>
public class RecurringJobBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRecurringJob _job;
    private readonly ILogger<RecurringJobBackgroundService> _logger;
    private readonly string _jobName;

    private System.Threading.Timer? _timer;

    private event Func<object, EventArgs, CancellationToken, Task>? TimerTriggered;

    private static readonly Meter _meter = new(
        name: typeof(RecurringJobBackgroundService).Assembly.GetName().Name!,
        version: typeof(RecurringJobBackgroundService).Assembly.GetName().Version?.ToString());

    private static readonly Histogram<double> _histogram =
        _meter.CreateHistogram<double>(
            name: $"{nameof(IRecurringJob)}.{nameof(ExecuteAsync)}".ToLower(),
            unit: "milliseconds",
            description: $"Histogram over duration and count of {nameof(IRecurringJob)}.{nameof(IRecurringJob.ExecuteAsync)}.");

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurringJobBackgroundService"/> class.
    /// </summary>
    /// <param name="job">The job.</param>
    /// <param name="serviceScopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger.</param>
    public RecurringJobBackgroundService(
        IRecurringJob job,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RecurringJobBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _job = job;
        _jobName = _job.GetType().Name;

        _logger.LogInformation("Started {className} with Job {job}",
            nameof(RecurringJobBackgroundService), _jobName);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new System.Threading.Timer(_ => TimerTriggered?.Invoke(this, EventArgs.Empty, stoppingToken),
           state: null,
           dueTime: _job.InitialDelay,
           period: _job.Interval);

        TimerTriggered += InternalExecuteAsync;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs the <see cref="IRecurringJob.ExecuteAsync"/> task.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    /// <param name="stoppingToken">The stopping token.</param>
    internal async Task InternalExecuteAsync(object sender, EventArgs eventArgs, CancellationToken stoppingToken)
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
        // RecurringJob from the ServiceProvider on every execution.
        if (_job.ServiceLifetime is not ServiceLifetime.Singleton)
        {
            _logger.LogDebug(
                "Fetching a {serviceLifetime} instance of {jobName} from the IServiceScopeFactory.",
                _job.ServiceLifetime, _jobName);

            using var scope = _serviceScopeFactory.CreateScope();

            var job = (IRecurringJob)scope.ServiceProvider.GetRequiredService(_job.GetType());

            await job.ExecuteAsync(stoppingToken).ConfigureAwait(false);

            _logger.LogDebug("Successfully executed the Job {jobName}", _jobName);

            return;
        }

        await _job.ExecuteAsync(stoppingToken).ConfigureAwait(false);
    }

    public override void Dispose()
    {
        _timer?.Dispose();

        TimerTriggered -= InternalExecuteAsync;

        GC.SuppressFinalize(this);

        base.Dispose();
    }
}
