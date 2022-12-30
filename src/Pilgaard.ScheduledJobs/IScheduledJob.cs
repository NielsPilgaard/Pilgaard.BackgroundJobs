using Microsoft.Extensions.DependencyInjection;
using Pilgaard.ScheduledJobs.Extensions;

namespace Pilgaard.ScheduledJobs;

/// <summary>
/// Implementing this interface and registering it in your <see cref="IServiceCollection"/> will give you a functional <see cref="IScheduledJob"/>.
/// <para>
/// Use <see cref="ServiceCollectionExtensions.AddScheduledJobs(IServiceCollection,System.Type[])"/> or one of it's overloads to register the <see cref="IScheduledJob"/> correctly.
/// </para>
/// </summary>
public interface IScheduledJob
{
    /// <summary>
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ExecuteAsync(CancellationToken cancellationToken = default);

    DateTime ScheduledTimeUtc { get; }
}
