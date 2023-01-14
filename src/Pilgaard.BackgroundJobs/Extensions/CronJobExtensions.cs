namespace Pilgaard.BackgroundJobs.Extensions;

public static class CronJobExtensions
{
    public static DateTime? GetNextOccurrence(this ICronJob cronJob)
        => cronJob.CronExpression.GetNextOccurrence(DateTime.UtcNow);

    public static IEnumerable<DateTime> GetOccurrences(this ICronJob cronJob, DateTime toUtc)
    {
        foreach (var occurrence in cronJob.CronExpression.GetOccurrences(DateTime.UtcNow, toUtc))
        {
            yield return occurrence;
        }
    }
}
