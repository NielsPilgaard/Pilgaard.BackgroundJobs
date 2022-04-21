using Cronos;

namespace Pilgaard.CronJobs;

public interface ICronJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
    CronExpression CronSchedule { get; }
}