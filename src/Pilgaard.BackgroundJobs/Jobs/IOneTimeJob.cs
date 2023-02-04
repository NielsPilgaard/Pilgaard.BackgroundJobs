namespace Pilgaard.BackgroundJobs;

/// <summary>
/// This interface represents a background job that runs once at the scheduled time, in UTC.
/// </summary>
public interface IOneTimeJob : IBackgroundJob
{
    /// <summary>
    /// Defines when <see cref="IBackgroundJob.RunJobAsync"/> should trigger.
    /// </summary>
    DateTime ScheduledTimeUtc { get; }
}
