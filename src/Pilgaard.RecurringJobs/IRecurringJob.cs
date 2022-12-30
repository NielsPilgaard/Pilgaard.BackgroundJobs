using Microsoft.Extensions.DependencyInjection;
using Pilgaard.RecurringJobs.Extensions;

namespace Pilgaard.RecurringJobs;

/// <summary>
/// Implementing this interface and registering it in your <see cref="IServiceCollection"/> will give you a functional CronJob.
/// <para>
/// Use <see cref="ServiceCollectionExtensions.AddRecurringJobs(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Type[])"/> or one of it's overloads to register the <see cref="IRecurringJob"/> correctly.
/// </para>
/// </summary>
public interface IRecurringJob
{
    /// <summary>
    /// This method is called whenever the <see cref="Interval"/> time has elapsed.
    /// <para>
    /// It is also called on startup.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ExecuteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The <see cref="TimeSpan"/> between each execution of the <see cref="IRecurringJob"/>.
    /// </summary>
    TimeSpan Interval { get; }

    /// <summary>
    /// The <see cref="ServiceLifetime"/> the Job should have.
    /// </summary>
    ServiceLifetime ServiceLifetime { get; }

    /// <summary>
    /// The initial delay before triggering <see cref="ExecuteAsync"/> the first time.
    /// <para>
    /// Set to <see cref="TimeSpan.Zero"/> to trigger <see cref="ExecuteAsync"/> on startup.
    /// </para>
    /// </summary>
    TimeSpan InitialDelay { get; }
}
