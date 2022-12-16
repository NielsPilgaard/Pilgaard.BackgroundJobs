using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.RecurringJobs.Configuration;

/// <summary>
/// Options that allow you to change the behaviour of your <see cref="IRecurringJob"/>s.
/// </summary>
public class RecurringJobOptions
{
    /// <summary>
    /// Gets or sets the service lifetime of the 
    /// <see cref="IRecurringJob"/>s passed into 
    /// <see cref="RecurringJobBackgroundService"/>s.
    /// </summary>
    /// <value>
    /// The service lifetime.
    /// </value>
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
}
