using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pilgaard.BackgroundJobs;

internal sealed class BackgroundJobService : IBackgroundJobService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly IBackgroundJobScheduler _backgroundJobScheduler;

    private static readonly Meter _meter = new(
        name: typeof(BackgroundJobService).Assembly.GetName().Name!,
        version: typeof(BackgroundJobService).Assembly.GetName().Version?.ToString());

    private static readonly Histogram<double> _histogram =
        _meter.CreateHistogram<double>(
            name: $"{nameof(BackgroundJobService)}.{nameof(RunJobAsync)}".ToLower(),
            unit: "milliseconds",
            description: $"Histogram over duration and count of {nameof(BackgroundJobService)}.{nameof(RunJobAsync)}.");


    public BackgroundJobService(
        IServiceScopeFactory scopeFactory,
        ILogger<BackgroundJobService> logger,
        IBackgroundJobScheduler backgroundJobScheduler)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _backgroundJobScheduler = backgroundJobScheduler ?? throw new ArgumentNullException(nameof(backgroundJobScheduler));
    }

    public async Task RunJobsAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("Scheduling background jobs.");

            var backgroundJobsToRun = _backgroundJobScheduler
                .GetBackgroundJobsAsync(cancellationToken)
                .WithCancellation(cancellationToken)
                .ConfigureAwait(false);

            await foreach (var registration in backgroundJobsToRun)
            {
                // Don't await, otherwise the timing will be off for frequently run jobs
                _ = Task.Run(async () => await RunJobAsync(registration, cancellationToken), cancellationToken);
            }
        }
    }

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
}
