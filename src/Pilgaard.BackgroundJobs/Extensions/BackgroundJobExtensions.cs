namespace Pilgaard.BackgroundJobs.Extensions;

public static class BackgroundJobExtensions
{
    public static DateTime? GetNextOccurrence(this IBackgroundJob backgroundJob) =>
        backgroundJob switch
        {
            ICronJob cronJob => cronJob.GetNextOccurrence(),
            IRecurringJob recurringJob => recurringJob.GetNextOccurrence(),
            IOneTimeJob oneTimeJob => oneTimeJob.GetNextOccurrence(),
            _ => throw new ArgumentOutOfRangeException(nameof(IBackgroundJob),
                $"Background job must implement either {nameof(ICronJob)}, {nameof(IRecurringJob)} or {nameof(IOneTimeJob)}")
        };

    public static IEnumerable<DateTime> GetOccurrences(this IBackgroundJob backgroundJob, DateTime toUtc) =>
        backgroundJob switch
        {
            ICronJob cronJob => cronJob.GetOccurrences(toUtc),
            IRecurringJob recurringJob => recurringJob.GetOccurrences(toUtc),
            IOneTimeJob oneTimeJob => oneTimeJob.GetOccurrences(toUtc),
            _ => throw new ArgumentOutOfRangeException(nameof(IBackgroundJob),
                $"Background job must implement either {nameof(ICronJob)}, {nameof(IRecurringJob)} or {nameof(IOneTimeJob)}")
        };
}
