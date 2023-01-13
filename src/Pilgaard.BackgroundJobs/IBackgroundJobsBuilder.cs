using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs;

public interface IBackgroundJobsBuilder
{
    IServiceCollection Services { get; }
    IBackgroundJobsBuilder Add(BackgroundJobRegistration registration);
}
