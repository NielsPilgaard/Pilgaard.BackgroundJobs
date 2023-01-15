namespace Pilgaard.BackgroundJobs.Extensions;

public static class OneTimeJobExtensions
{
    public static DateTime? GetNextOccurrence(this IOneTimeJob oneTimeJob)
    {
        if (DateTime.UtcNow > oneTimeJob.ScheduledTimeUtc)
        {
            return null;
        }

        return oneTimeJob.ScheduledTimeUtc;
    }

    public static IEnumerable<DateTime> GetOccurrences(this IOneTimeJob oneTimeJob, DateTime toUtc)
    {
        // If toUtc is less than the scheduled time, it's not within the range of occurrences to return
        if (toUtc < oneTimeJob.ScheduledTimeUtc)
        {
            return new DateTime[] { };
        }

        // If the current timer is higher than the scheduled time, there is no next occurrence
        if (DateTime.UtcNow > oneTimeJob.ScheduledTimeUtc)
        {
            return new DateTime[] { };
        }

        return new[] { oneTimeJob.ScheduledTimeUtc };
    }
}
