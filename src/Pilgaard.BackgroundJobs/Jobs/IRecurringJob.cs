namespace Pilgaard.BackgroundJobs;

/// <summary>
/// This interface represents a background job that runs at a specified interval.
/// </summary>
public interface IRecurringJob : IBackgroundJob
{
    /// <summary>
    /// The interval at which <see cref="IBackgroundJob.RunJobAsync"/> triggers.
    /// </summary>
    TimeSpan Interval { get; }
}
