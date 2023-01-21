namespace Pilgaard.BackgroundJobs;

internal readonly record struct BackgroundJobOccurrence(
    DateTime Occurrence,
    BackgroundJobRegistration BackgroundJobRegistration)
{
    public DateTime Occurrence { get; } = Occurrence;
    public BackgroundJobRegistration BackgroundJobRegistration { get; } = BackgroundJobRegistration;
}
