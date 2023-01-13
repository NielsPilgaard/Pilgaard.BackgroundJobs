namespace Pilgaard.BackgroundJobs;

/// <summary>
/// A simple implementation of <see cref="IOneTimeJob"/> which uses a provided delegate to
/// implement the job.
/// </summary>
internal sealed class DelegateOneTimeJob : IOneTimeJob
{
    private readonly Func<CancellationToken, Task> _job;
    public DateTime ScheduledTimeUtc { get; }

    public DelegateOneTimeJob(Func<CancellationToken, Task> job, DateTime scheduledTimeUtc)
    {
        _job = job ?? throw new ArgumentNullException(nameof(job));
        ScheduledTimeUtc = scheduledTimeUtc;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
        => _job(cancellationToken);

}
