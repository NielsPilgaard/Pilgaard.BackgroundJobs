using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// A service which can be used to run background jobs registered in the application.
/// </summary>
/// <remarks>
/// <para>
/// The default implementation of <see cref="IBackgroundJobService"/> is registered in the dependency
/// injection container as a singleton service by calling
/// <see cref="ServiceCollectionExtensions.AddBackgroundJobs(IServiceCollection)"/>
/// </para>
/// <para>
/// The <see cref="IBackgroundJobsBuilder"/> returned by
/// <see cref="ServiceCollectionExtensions.AddBackgroundJobs(IServiceCollection)"/>
/// provides a convenience API for registering health checks.
/// </para>
/// <para>
/// <see cref="IBackgroundJob"/> implementations can be registered through extension methods provided by
/// <see cref="IBackgroundJobsBuilder"/>.
/// </para>
/// </remarks>
public interface IBackgroundJobService
{
    /// <summary>
    /// Runs all the background jobs in the application.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>
    /// which can be used to cancel the background jobs.</param>
    /// <returns>
    /// A <see cref="Task"/> which will complete when all background jobs have been run, and there are no more occurrences of any of them.
    /// </returns>
    Task RunJobsAsync(CancellationToken cancellationToken = default);
}
