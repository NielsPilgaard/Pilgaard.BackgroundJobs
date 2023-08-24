namespace Pilgaard.BackgroundJobs;

/// <summary>
/// This interface represents a background job that runs at a specified interval, after an initial delay.
/// </summary>
public interface IRecurringJobWithInitialDelay : IRecurringJob
{
    /// <summary>
    /// The initial delay before triggering <see cref="IBackgroundJob.RunJobAsync"/> the first time.
    /// <para>
    /// Setting this to <see cref="TimeSpan.Zero"/> triggers it immediately on startup.
    /// </para>
    /// </summary>
    TimeSpan InitialDelay { get; }
}
