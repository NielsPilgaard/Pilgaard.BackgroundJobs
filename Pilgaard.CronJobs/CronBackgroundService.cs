using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pilgaard.CronJobs;

public class CronBackgroundService<TCronJob> : BackgroundService
    where TCronJob : ICronJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TCronJob _cronJob;
    private readonly CronJobOptions _options;
    private readonly CronExpression _cronSchedule;

    public CronBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        Action<CronJobOptions>? configuration = null)
    {
        _options = new CronJobOptions();
        configuration?.Invoke(_options);

        _serviceScopeFactory = serviceScopeFactory;

        // Lookup CronJob to get it's schedule without compromising its lifecycle
        // If lifetime is set to Singleton, the CronJob remains un-disposed.
        using var scope = _serviceScopeFactory.CreateScope();
        _cronJob = scope.ServiceProvider.GetRequiredService<TCronJob>();
        _cronSchedule = _cronJob.CronSchedule;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var nextTaskOccurrence = GetNextOccurrence();

        while (nextTaskOccurrence is not null &&
               stoppingToken.IsCancellationRequested is false)
        {
            await PerformTaskOnNextOccurrence(nextTaskOccurrence, stoppingToken);

            nextTaskOccurrence = GetNextOccurrence();
        }
    }

    public virtual async Task PerformTaskOnNextOccurrence(
        DateTime? nextTaskExecution,
        CancellationToken stoppingToken)
    {
        var dormantTimeSpan = DateTime.UtcNow - nextTaskExecution!.Value;

        await Task.Delay(dormantTimeSpan, stoppingToken);

        // If ServiceLifetime is Transient or Scoped, we need to re-fetch the
        // CronJob from the ServiceProvider on every execution.
        if (_options.ServiceLifetime is not ServiceLifetime.Singleton)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var cronService = scope.ServiceProvider.GetRequiredService<TCronJob>();

            await cronService.ExecuteAsync(stoppingToken);
            return;
        }

        await _cronJob.ExecuteAsync(stoppingToken);
    }

    public virtual DateTime? GetNextOccurrence() =>
        _cronSchedule.GetNextOccurrence(DateTime.UtcNow, _options.TimeZoneInfo);
}