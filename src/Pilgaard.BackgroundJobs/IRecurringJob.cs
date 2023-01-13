namespace Pilgaard.BackgroundJobs;

public interface IRecurringJob : IBackgroundJob
{
    TimeSpan Interval { get; }
}