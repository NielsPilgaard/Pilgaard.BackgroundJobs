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
    /// This method is called whenever <see cref="CronSchedule"/> triggers.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
