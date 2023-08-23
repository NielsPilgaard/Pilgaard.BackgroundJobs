using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// Extension methods for the <see cref="IBackgroundJobsBuilder"/>.
/// </summary>
public static class BackgroundJobsBuilderExtensions
{
    /// <summary>
    /// Adds a background job of type <typeparamref name="TJob"/> to the <see cref="IBackgroundJobsBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="name">The name to use for the job. Uses the name of <typeparamref name="TJob"/> if <paramref name="name"/> is <c>null</c>.</param>
    /// <param name="timeout">The timeout of the job, defaults to no timeout.</param>
    /// <exception cref="ArgumentNullException">Throws if either <paramref name="builder"/> or <paramref name="name"/> is <c>null</c></exception>
    /// <returns>The <see cref="IBackgroundJobsBuilder"/> for further chaining.</returns>
    public static IBackgroundJobsBuilder AddJob<TJob>(
        this IBackgroundJobsBuilder builder,
        string? name = null,
        TimeSpan? timeout = default) where TJob : class, IBackgroundJob
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.Add(new BackgroundJobRegistration(GetServiceOrCreateInstance, name ?? typeof(TJob).Name, timeout, typeof(TJob).ImplementsRecurringJob()));

        static TJob GetServiceOrCreateInstance(IServiceProvider serviceProvider) =>
            ActivatorUtilities.GetServiceOrCreateInstance<TJob>(serviceProvider);
    }
}
