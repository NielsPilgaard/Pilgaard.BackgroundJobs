using Cronos;

namespace Pilgaard.BackgroundJobs;

public interface ICronJob : IBackgroundJob
{
    CronExpression CronExpression { get; }
}
