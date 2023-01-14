namespace Pilgaard.BackgroundJobs;

public interface IBackgroundJob
{
    Task RunJobAsync(CancellationToken cancellationToken = default);

    DateTime? GetNextOccurrence() =>
        this switch
        {
            ICronJob cronJob => cronJob.GetNextOccurrence(),
            IRecurringJob recurringJob => recurringJob.GetNextOccurrence(),
            IOneTimeJob oneTimeJob => oneTimeJob.GetNextOccurrence(),
            _ => throw new ArgumentOutOfRangeException(nameof(IBackgroundJob),
                $"Background job must implement either {nameof(ICronJob)}, {nameof(IRecurringJob)} or {nameof(IOneTimeJob)}")
        };

    IEnumerable<DateTime?> GetOccurrences(DateTime toUtc) =>
        this switch
        {
            ICronJob cronJob => cronJob.GetOccurrences(toUtc),
            IRecurringJob recurringJob => recurringJob.GetOccurrences(toUtc),
            IOneTimeJob oneTimeJob => oneTimeJob.GetOccurrences(toUtc),
            _ => throw new ArgumentOutOfRangeException(nameof(IBackgroundJob),
                $"Background job must implement either {nameof(ICronJob)}, {nameof(IRecurringJob)} or {nameof(IOneTimeJob)}")
        };
}
