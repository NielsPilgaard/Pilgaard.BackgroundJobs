namespace Pilgaard.BackgroundJobs;

internal interface IBackgroundJobScheduler
{
    IAsyncEnumerable<BackgroundJobRegistration> GetBackgroundJobsAsync(CancellationToken cancellationToken);
}
