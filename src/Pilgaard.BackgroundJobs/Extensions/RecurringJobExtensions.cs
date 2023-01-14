namespace Pilgaard.BackgroundJobs.Extensions;

public static class RecurringJobExtensions
{
    public static DateTime GetNextOccurrence(this IRecurringJob recurringJob)
        => DateTime.UtcNow.Subtract(recurringJob.Interval);

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
