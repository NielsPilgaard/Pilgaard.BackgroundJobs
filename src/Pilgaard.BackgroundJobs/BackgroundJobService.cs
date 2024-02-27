using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// A service which can be used to run background jobs registered in the application.
/// </summary>
/// <remarks>
/// <para>
/// The default implementation of <see cref="IBackgroundJobService"/> is registered in the dependency
/// injection container as a singleton service by calling
/// <see cref="ServiceCollectionExtensions.AddBackgroundJobs(IServiceCollection)"/>
/// </para>
/// <para>
/// The <see cref="IBackgroundJobsBuilder"/> returned by
/// <see cref="ServiceCollectionExtensions.AddBackgroundJobs(IServiceCollection)"/>
/// provides a convenience API for registering health checks.
/// </para>
/// <para>
/// <see cref="IBackgroundJob"/> implementations can be registered through extension methods provided by
/// <see cref="IBackgroundJobsBuilder"/>.
/// </para>
/// </remarks>
internal sealed class BackgroundJobService : IBackgroundJobService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly IBackgroundJobScheduler _backgroundJobScheduler;
    private readonly BackgroundJobServiceOptions _backgroundJobServiceOptions;

    private event Func<object, EventArgs, BackgroundJobRegistration, CancellationToken, Task>? RecurringJobTimerTriggered;
    private static readonly List<IDisposable> _recurringJobTimers = [];

    private static readonly Meter _meter = new(
        name: typeof(BackgroundJobService).Assembly.GetName().Name!,
        version: typeof(BackgroundJobService).Assembly.GetName().Version?.ToString());

    private static readonly Histogram<double> _histogram =
        _meter.CreateHistogram<double>(
            name: $"{nameof(BackgroundJobService)}.{nameof(RunJobAsync)}".ToLower(),
            unit: "milliseconds",
            description: $"Histogram over duration and count of {nameof(BackgroundJobService)}.{nameof(RunJobAsync)}.");


    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundJobService"/> class
    /// </summary>
    /// <param name="scopeFactory">The factory used when constructing background jobs.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="backgroundJobScheduler">The background job scheduler used to retrieve jobs when they should be run.</param>
    /// <param name="backgroundJobServiceOptions">The options holding all <see cref="BackgroundJobRegistration"/>s, used to determine whether the <see cref="BackgroundJobService"/> should begin polling for occurrences or not.</param>
    public BackgroundJobService(
        IServiceScopeFactory scopeFactory,
        ILogger<BackgroundJobService> logger,
        IBackgroundJobScheduler backgroundJobScheduler,
        IOptions<BackgroundJobServiceOptions> backgroundJobServiceOptions)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _backgroundJobScheduler = backgroundJobScheduler ?? throw new ArgumentNullException(nameof(backgroundJobScheduler));
        _backgroundJobServiceOptions = backgroundJobServiceOptions.Value;
    }

    /// <summary>
    /// Runs all the background jobs in the application.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>
    /// which can be used to cancel the background jobs.</param>
    /// <returns>
    /// A <see cref="Task"/> which will complete when all background jobs have been run, and there are no more occurrences of them.
    /// </returns>
    public async Task RunJobsAsync(CancellationToken cancellationToken = default)
    {
        ScheduleRecurringJobs(cancellationToken);

        if (_backgroundJobServiceOptions
                .Registrations
                .Count(registration => registration.IsRecurringJob is false) is 0)
        {
            _logger.LogInformation("No {OneTimeJob} or {CronJob} have been registered.",
                nameof(IOneTimeJob), nameof(ICronJob));
            return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("Scheduling background jobs.");

            var backgroundJobsToRun = _backgroundJobScheduler
                .GetBackgroundJobsAsync(cancellationToken)
                .ConfigureAwait(false);

            await foreach (var registration in backgroundJobsToRun)
            {
                // Don't await, otherwise the timing will be off for frequently run jobs
                _ = Task.Run(async () => await RunJobAsync(registration, cancellationToken), cancellationToken);
            }
        }
    }

    internal void ScheduleRecurringJobs(CancellationToken cancellationToken)
    {
        var recurringJobRegistrations = _backgroundJobScheduler.GetRecurringJobs();
        if (recurringJobRegistrations.Any())
        {
            RecurringJobTimerTriggered += RunRecurringJobAsync;
        }

        using var scope = _scopeFactory.CreateScope();
        foreach (var jobRegistration in recurringJobRegistrations)
        {
            if (jobRegistration.Factory(scope.ServiceProvider) is not IRecurringJob recurringJob)
            {
                _logger.LogError("Failed to schedule recurring job {@jobRegistration}. " +
                                 "It does not implement {recurringJobInterface}",
                    jobRegistration, typeof(IRecurringJob));
                continue;
            }

            var dueTime = recurringJob switch
            {
                IRecurringJobWithInitialDelay recurringJobWithInitialDelay => recurringJobWithInitialDelay.InitialDelay,
                _ => recurringJob.Interval
            };

            var recurringJobTimer = new System.Threading.Timer(_ => RecurringJobTimerTriggered?.Invoke(this, EventArgs.Empty, jobRegistration, cancellationToken),
                state: null,
                dueTime: dueTime,
                period: recurringJob.Interval);

            _recurringJobTimers.Add(recurringJobTimer);

            _logger.LogInformation("RecurringJob {jobName} has been scheduled to run every {interval}. " +
                                   "The first run will be in {dueTime}",
                jobRegistration.Name, recurringJob.Interval, dueTime);
        }
    }

    /// <summary>
    /// Runs the recurring job.
    /// </summary>
    /// <param name="sender">The sender. This is not used.</param>
    /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data. This is not used.</param>
    /// <param name="registration">The background job registration.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> which can be used to cancel the background job.</param>
    internal async Task RunRecurringJobAsync(object sender, EventArgs eventArgs, BackgroundJobRegistration registration, CancellationToken cancellationToken)
        => await RunJobAsync(registration, cancellationToken);

    /// <summary>
    /// Constructs the background job using <see cref="BackgroundJobRegistration.Factory"/> and runs it.
    /// </summary>
    /// <param name="registration">The background job registration.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> which can be used to cancel the background job.</param>
    /// <returns></returns>
    internal async Task RunJobAsync(BackgroundJobRegistration registration, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var backgroundJob = registration.Factory(scope.ServiceProvider);

        // If the background job does things like make Database queries using EF or backend HTTP calls,
        // it may be valuable to know that logs it generates are part of a background job, so we start a scope.
        using var logScope = _logger.BeginScope(registration.Name);

        using var histogram = _histogram.NewTimer(tags: new KeyValuePair<string, object?>("job_name", registration.Name));

        _logger.LogDebug("Running background job {jobName}", registration.Name);

        CancellationTokenSource? timeoutCancellationTokenSource = null;
        try
        {
            var jobCancellationToken = cancellationToken;
            if (registration.Timeout > TimeSpan.Zero)
            {
                timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCancellationTokenSource.CancelAfter(registration.Timeout);
                jobCancellationToken = timeoutCancellationTokenSource.Token;
            }

            await backgroundJob.RunJobAsync(jobCancellationToken);

            _logger.LogDebug("Background job {jobName} completed after {duration}ms", registration.Name, histogram.ObserveDuration().TotalMilliseconds);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(exception,
                "A timeout occurred after {duration}ms while running background job {backgroundJob}.",
                histogram.ObserveDuration().TotalMilliseconds,
                registration.Name);
        }
        // Allow cancellation to propagate if it's not a timeout.
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception,
                "An exception occurred after {duration}ms while running background job {backgroundJob}.", histogram.ObserveDuration().TotalMilliseconds,
                registration.Name);
        }
        finally
        {
            timeoutCancellationTokenSource?.Dispose();
        }
    }

    public void Dispose()
    {
        foreach (var disposable in _recurringJobTimers)
        {
            disposable.Dispose();
        }
    }
}
