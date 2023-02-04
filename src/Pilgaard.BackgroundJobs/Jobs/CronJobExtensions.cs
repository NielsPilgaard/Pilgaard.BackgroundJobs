namespace Pilgaard.BackgroundJobs;

public static class CronJobExtensions
{
    /// <summary>
    /// Get the next occurrence of the cron job.
    /// </summary>
    /// <param name="cronJob">The cron job to get the next occurrence of.</param>
    /// <returns><see cref="DateTime"/> of the next occurrence, or <c>null</c> if none.</returns>
    public static DateTime? GetNextOccurrence(this ICronJob cronJob)
        => cronJob.CronExpression.GetNextOccurrence(DateTime.UtcNow);

    /// <summary>
    /// Get all occurrences of the cron job up to a certain date.
    /// </summary>
    /// <param name="cronJob">The cron job to get the occurrences of.</param>
    /// <param name="toUtc">The date up to which to get occurrences.</param>
    /// <returns><see cref="IEnumerable{T}"/> of all occurrences.</returns>
    public static IEnumerable<DateTime> GetOccurrences(this ICronJob cronJob, DateTime toUtc)
    {
        foreach (var occurrence in cronJob.CronExpression.GetOccurrences(DateTime.UtcNow, toUtc))
        {
            yield return occurrence;
        }
    }
}
