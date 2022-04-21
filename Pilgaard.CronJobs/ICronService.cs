using Cronos;

namespace Pilgaard.CronJobs;

public interface ICronService
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
    CronExpression CronSchedule { get; }
}