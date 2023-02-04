using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// Provides extension methods for registering <see cref="IBackgroundJob"/> in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required to schedule, validate and execute background jobs.
    /// </summary>
    /// <remarks>
    /// This operation is idempotent - multiple invocations will still only result in a single
    /// <see cref="IBackgroundJobService"/> instance in the <see cref="IServiceCollection"/>. It can be invoked
    /// multiple times in order to get access to the <see cref="IBackgroundJobsBuilder"/> in multiple places.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IBackgroundJobService"/> to.</param>
    /// <returns>An instance of <see cref="IBackgroundJobsBuilder"/> from which background jobs can be registered.</returns>
    public static IBackgroundJobsBuilder AddBackgroundJobs(this IServiceCollection services)
    {
        services.TryAddTransient<IRegistrationValidator, RegistrationValidator>();

        services.TryAddSingleton<IBackgroundJobService, BackgroundJobService>();
        services.TryAddSingleton<IBackgroundJobScheduler, BackgroundJobScheduler>();

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, BackgroundJobHostedService>());

        return new BackgroundJobsBuilder(services);
    }
}
