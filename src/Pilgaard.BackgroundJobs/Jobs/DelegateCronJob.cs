using Cronos;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// A simple implementation of <see cref="ICronJob"/> which uses a provided delegate to
/// implement the job.
/// </summary>
internal sealed class DelegateCronJob : ICronJob
{
    private readonly Func<CancellationToken, Task> _job;
    public CronExpression CronExpression { get; }

    public DelegateCronJob(Func<CancellationToken, Task> job, CronExpression cronExpression)
    {
        _job = job ?? throw new ArgumentNullException(nameof(job));
        CronExpression = cronExpression;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
        => _job(cancellationToken);
}
