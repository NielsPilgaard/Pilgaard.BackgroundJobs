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

    public static IEnumerable<DateTime?> GetOccurrences(this IOneTimeJob oneTimeJob)
        => new[] { oneTimeJob.GetNextOccurrence() };
}
