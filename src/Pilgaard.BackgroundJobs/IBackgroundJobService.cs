namespace Pilgaard.BackgroundJobs;

public interface IBackgroundJobService
{
    Task RunJobsAsync(Func<BackgroundJobRegistration, bool>? predicate, CancellationToken cancellationToken = default);
}
