namespace Pilgaard.BackgroundJobs;

public sealed class BackgroundJobRegistration
{
    public BackgroundJobRegistration(IBackgroundJob instance,
        string? name = null,
        TimeSpan? timeout = null,
        IEnumerable<string>? tags = null)
    {
        if (timeout <= TimeSpan.Zero && timeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        Name = name ?? throw new ArgumentNullException(nameof(name));

        Factory = (_) => instance;

        Timeout = timeout ?? System.Threading.Timeout.InfiniteTimeSpan;

        Tags = new HashSet<string>(tags ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
    }

    public BackgroundJobRegistration(Func<IServiceProvider, IBackgroundJob> factory,
        string? name = null,
        TimeSpan? timeout = null,
        IEnumerable<string>? tags = null)
    {
        if (timeout <= TimeSpan.Zero && timeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        Name = name ?? throw new ArgumentNullException(nameof(name));

        Factory = factory ?? throw new ArgumentNullException(nameof(factory));

        Timeout = timeout ?? System.Threading.Timeout.InfiniteTimeSpan;

        Tags = new HashSet<string>(tags ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets a delegate used to create the <see cref="IBackgroundJob"/> instance.
    /// </summary>
    public Func<IServiceProvider, IBackgroundJob> Factory { get; }

    /// <summary>
    /// Gets the job name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the timeout used for the job.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Gets a list of tags that can be used for filtering jobs.
    /// </summary>
    public ISet<string> Tags { get; }

    /// <summary>
    /// Gets the type of the job.
    /// </summary>
    public BackgroundJobType BackgroundJobType { get; }
}

public enum BackgroundJobType
{
    Cron = 0,
    Recurring = 1,
    Scheduled = 2
}
