using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.ScheduledJobs.Configuration;

/// <summary>
/// Options that allow you to change the behaviour of your <see cref="IScheduledJob"/>s.
/// </summary>
public class ScheduledJobOptions
{
    /// <summary>
    /// Gets or sets the service lifetime of the 
    /// <see cref="IScheduledJob"/>s passed into 
    /// <see cref="ScheduledJobBackgroundService"/>s.
    /// </summary>
    /// <value>
    /// The service lifetime.
    /// </value>
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
}
