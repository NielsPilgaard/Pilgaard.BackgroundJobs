using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pilgaard.CronJobs;

public class CronBackgroundService<TCronService> : BackgroundService
    where TCronService : ICronService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TCronService _cronService;
    private readonly CronBackgroundServiceOptions _options;
    private readonly CronExpression _cronSchedule;

    public CronBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        Action<CronBackgroundServiceOptions>? configuration = null)
    {
        _options = new CronBackgroundServiceOptions();
        configuration?.Invoke(_options);

        _serviceScopeFactory = serviceScopeFactory;

        // Lookup CronService to get it's schedule without compromising its lifecycle
        // If lifetime is set to Singleton, the CronService remains un-disposed.
        using var scope = _serviceScopeFactory.CreateScope();
        _cronService = scope.ServiceProvider.GetRequiredService<TCronService>();
        _cronSchedule = _cronService.CronSchedule;
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
        // CronService from the ServiceProvider on every execution.
        if (_options.ServiceLifetime is not ServiceLifetime.Singleton)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var cronService = scope.ServiceProvider.GetRequiredService<TCronService>();

            await cronService.ExecuteAsync(stoppingToken);
            return;
        }

        await _cronService.ExecuteAsync(stoppingToken);
    }

    public virtual DateTime? GetNextOccurrence() =>
        _cronSchedule.GetNextOccurrence(DateTime.UtcNow, _options.TimeZoneInfo);
}