namespace Pilgaard.BackgroundJobs;

public static class OneTimeJobExtensions
{
    /// <summary>
    /// Get the next occurrence of the one-time job.
    /// </summary>
    /// <param name="oneTimeJob">The one-time job to get the next occurrence of.</param>
    /// <returns>The next (and only) occurrence of the one-time job, or <c>null</c> if the occurrence is in the past.</returns>
    public static DateTime? GetNextOccurrence(this IOneTimeJob oneTimeJob)
    {
        if (DateTime.UtcNow > oneTimeJob.ScheduledTimeUtc)
            return null;

        return oneTimeJob.ScheduledTimeUtc;
    }

    /// <summary>
    /// Get the next occurrence of the one-time job, as an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="oneTimeJob">The one-time job to get the next occurrence of.</param>
    /// <param name="toUtc">The date up to which to get occurrences.</param>
    /// <returns>An array of the next (and only) occurrence of the one-time job, or <see cref="Enumerable.Empty{TResult}"/> if the occurrence is in the past.</returns>
    public static IEnumerable<DateTime> GetOccurrences(this IOneTimeJob oneTimeJob, DateTime toUtc)
    {
        // If toUtc is less than the scheduled time, it's not within the range of occurrences to return
        if (toUtc < oneTimeJob.ScheduledTimeUtc)
            return Enumerable.Empty<DateTime>();

        // If the current time is higher than the scheduled time, there is no next occurrence
        if (DateTime.UtcNow > oneTimeJob.ScheduledTimeUtc)
            return Enumerable.Empty<DateTime>();

        return new[] { oneTimeJob.ScheduledTimeUtc };
    }
}
