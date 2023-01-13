using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pilgaard.BackgroundJobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IBackgroundJobsBuilder AddBackgroundJobs(this IServiceCollection services)
    {
        services.TryAddSingleton<IBackgroundJobService, DefaultBackgroundJobService>();
        return new BackgroundJobsBuilder(services);
    }
}
