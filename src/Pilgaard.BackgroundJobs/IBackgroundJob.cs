namespace Pilgaard.BackgroundJobs;

public interface IBackgroundJob
{
    Task RunJobAsync(CancellationToken cancellationToken = default);
}
