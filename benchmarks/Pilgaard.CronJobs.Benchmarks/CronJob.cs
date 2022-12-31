using Cronos;
using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.CronJobs.Benchmarks;

public class CronJob : ICronJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public CronExpression CronSchedule => CronExpression.Parse("* * * * *");
    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}
