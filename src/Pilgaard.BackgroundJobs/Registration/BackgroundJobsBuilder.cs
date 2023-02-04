using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// The builder for adding background jobs.
/// </summary>
internal sealed class BackgroundJobsBuilder : IBackgroundJobsBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundJobsBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public BackgroundJobsBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Adds a <see cref="BackgroundJobRegistration"/> to the service collection.
    /// </summary>
    /// <param name="registration">The background job registration to add.</param>
    /// <returns>The current <see cref="BackgroundJobsBuilder"/> instance for further chaining.</returns>
    public IBackgroundJobsBuilder Add(BackgroundJobRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        Services.Configure<BackgroundJobServiceOptions>(options => options.Registrations.Add(registration));

        return this;
    }
}
