namespace Pilgaard.BackgroundJobs;

/// <summary>
/// A simple implementation of <see cref="IRecurringJobWithInitialDelay"/> which uses a provided delegate to
/// implement the job.
/// </summary>
internal sealed class DelegateRecurringJobWithInitialDelay : IRecurringJobWithInitialDelay
{
    private readonly Func<CancellationToken, Task> _job;

    public TimeSpan Interval { get; }

    public TimeSpan InitialDelay { get; }

    public DelegateRecurringJobWithInitialDelay(Func<CancellationToken, Task> job, TimeSpan interval, TimeSpan initialDelay)
    {
        _job = job ?? throw new ArgumentNullException(nameof(job));
        Interval = interval;
        InitialDelay = initialDelay;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
        => _job(cancellationToken);

}