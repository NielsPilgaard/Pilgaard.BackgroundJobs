using Cronos;

namespace Pilgaard.CronJobs.Benchmarks;

public class CronJob : ICronJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public CronExpression CronSchedule => CronExpression.Parse("* * * * *");
}