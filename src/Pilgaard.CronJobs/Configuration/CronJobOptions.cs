using Cronos;
using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.CronJobs.Configuration;

/// <summary>
/// Options that allow you to change the behaviour of your <see cref="ICronJob"/>s.
/// </summary>
public class CronJobOptions
{
    /// <summary>
    /// Gets or sets the service lifetime of the 
    /// <see cref="ICronJob"/>s passed into 
    /// <see cref="CronBackgroundService"/>s.
    /// </summary>
    /// <value>
    /// The service lifetime.
    /// </value>
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    /// The time zone to use for calculating the occurrences of <see cref="CronExpression"/>s.
    /// </summary>
    /// <value>
    /// The time zone information.
    /// </value>
    public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
}