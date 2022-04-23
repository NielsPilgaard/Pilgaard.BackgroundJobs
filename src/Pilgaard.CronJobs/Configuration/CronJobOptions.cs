using Cronos;
using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.CronJobs.Configuration;

public class CronJobOptions
{
    /// <summary>
    /// Whether to include seconds when parsing Cron strings.
    /// </summary>
    /// <value>
    /// The cron format.
    /// </value>
    public CronFormat CronFormat { get; set; } = CronFormat.Standard;

    /// <summary>
    /// Gets or sets the service lifetime of the 
    /// <see cref="ICronJob"/> passed into the 
    /// <see cref="CronBackgroundService"/>.
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