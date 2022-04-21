
using Cronos;

namespace Pilgaard.CronJobs;

public class CronJob : ICronJob
{
    public CronJob(CronExpression cronExpression)
    {
        CronSchedule = cronExpression;
    }
    public CronJob(string cronExpression)
    {
        CronSchedule = CronExpression.Parse(cronExpression);
    }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public CronExpression CronSchedule { get; }
}