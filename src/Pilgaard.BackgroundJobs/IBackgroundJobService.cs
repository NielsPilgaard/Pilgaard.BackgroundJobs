namespace Pilgaard.BackgroundJobs;

public interface IBackgroundJobService
{
    Task RunJobsAsync(CancellationToken cancellationToken = default);
}
