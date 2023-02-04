using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// A builder used to register background jobs.
/// </summary>
public interface IBackgroundJobsBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where <see cref="IBackgroundJob"/> instances are registered.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Adds the <paramref name="registration"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="registration"></param>
    /// <returns></returns>
    IBackgroundJobsBuilder Add(BackgroundJobRegistration registration);
}
