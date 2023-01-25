using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pilgaard.BackgroundJobs;

public static class ServiceCollectionExtensions
{
    public static IBackgroundJobsBuilder AddBackgroundJobs(this IServiceCollection services)
    {
        services.TryAddSingleton<IBackgroundJobService, BackgroundJobService>();
        services.TryAddSingleton<IRegistrationValidator, RegistrationValidator>();
        services.TryAddSingleton<IBackgroundJobScheduler, BackgroundJobScheduler>();
        services.AddHostedService<BackgroundJobHostedService>();
        return new BackgroundJobsBuilder(services);
    }
}
