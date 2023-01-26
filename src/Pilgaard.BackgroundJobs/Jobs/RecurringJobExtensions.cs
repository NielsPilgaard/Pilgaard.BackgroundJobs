namespace Pilgaard.BackgroundJobs;

public static class RecurringJobExtensions
{
    /// <summary>
    /// Get the next occurrence of the recurring job.
    /// </summary>
    /// <param name="recurringJob">The recurring job to get the next occurrence of.</param>
    /// <returns><see cref="DateTime"/> of the next occurrence.</returns>
    public static DateTime GetNextOccurrence(this IRecurringJob recurringJob)
        => DateTime.UtcNow.Subtract(recurringJob.Interval);

    /// <summary>
    /// Get all occurrences of the recurring job up to a certain date.
    /// </summary>
    /// <param name="recurringJob">The recurring job to get the occurrences of.</param>
    /// <param name="toUtc">The date up to which to get occurrences.</param>
    /// <returns><see cref="IEnumerable{T}"/> of all occurrences.</returns>
    public static IEnumerable<DateTime> GetOccurrences(this IRecurringJob recurringJob, DateTime toUtc)
    {
        var nextOccurrence = DateTime.UtcNow.Add(recurringJob.Interval);
        while (nextOccurrence < toUtc)
        {
            yield return nextOccurrence;
            nextOccurrence = nextOccurrence.Add(recurringJob.Interval);
        }
    }
}
