using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs;

internal sealed class BackgroundJobsBuilder : IBackgroundJobsBuilder
{
    public BackgroundJobsBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public IBackgroundJobsBuilder Add(BackgroundJobRegistration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        Services.Configure<BackgroundJobServiceOptions>(options => options.Registrations.Add(registration));

        return this;
    }
}
