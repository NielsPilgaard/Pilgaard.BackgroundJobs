
using Cronos;

namespace Pilgaard.CronJobs;

public class CronService : ICronService
{
    public CronService(CronExpression cronExpression)
    {
        CronSchedule = cronExpression;
    }
    public CronService(string cronExpression)
    {
        CronSchedule = CronExpression.Parse(cronExpression);
    }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public CronExpression CronSchedule { get; }
}