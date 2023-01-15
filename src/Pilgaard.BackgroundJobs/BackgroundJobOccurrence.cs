namespace Pilgaard.BackgroundJobs;

internal readonly record struct BackgroundJobOccurrence(DateTime Occurrence, IBackgroundJob BackgroundJob)
{
    public DateTime Occurrence { get; } = Occurrence;
    public IBackgroundJob BackgroundJob { get; } = BackgroundJob;
}
