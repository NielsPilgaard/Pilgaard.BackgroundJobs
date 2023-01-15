namespace Pilgaard.BackgroundJobs;

internal interface IBackgroundJobScheduler
{
    IAsyncEnumerable<IBackgroundJob> GetBackgroundJobsAsync(DateTime toUtc, CancellationToken cancellationToken = default);
}
