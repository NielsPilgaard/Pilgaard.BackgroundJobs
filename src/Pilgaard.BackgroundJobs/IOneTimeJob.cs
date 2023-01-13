namespace Pilgaard.BackgroundJobs;

public interface IOneTimeJob : IBackgroundJob
{
    DateTime ScheduledTimeUtc { get; }
}
