using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs.Extensions;

public static class BackgroundJobsBuilderExtensions
{
    public static IBackgroundJobsBuilder AddJob<TJob>(
        this IBackgroundJobsBuilder builder,
        string name,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = default) where TJob : class, IBackgroundJob
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return builder.Add(new BackgroundJobRegistration(GetServiceOrCreateInstance, name, timeout, tags));

        static TJob GetServiceOrCreateInstance(IServiceProvider serviceProvider) =>
            ActivatorUtilities.GetServiceOrCreateInstance<TJob>(serviceProvider);
    }
}
