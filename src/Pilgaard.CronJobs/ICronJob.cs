using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.CronJobs.Extensions;

namespace Pilgaard.CronJobs;

/// <summary>
/// Implementing this interface and registering it in your <see cref="IServiceCollection"/> will give you a functional CronJob.
/// <para>
/// Use <see cref="ServiceCollectionExtensions.AddCronJobs(IServiceCollection, Type[])"/> or one of it's overloads to register the <see cref="ICronJob"/> correctly.
/// </para>
/// </summary>
public interface ICronJob
{
    /// <summary>
    /// This method is called whenever <see cref="CronSchedule"/> triggers.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ExecuteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Defines when <see cref="ExecuteAsync"/> should trigger.
    /// <para>
    /// We recommend that you use a cron expression editor to make your cron expression string.
    /// </para>
    /// </summary>
    /// <value>
    /// The cron schedule.
    /// </value>
    /// <remarks>
    ///     <example>
    ///     <para>
    ///         Example that executes every minute: 
    ///     </para>
    ///     <para>
    ///         <c>
    ///             <see langword="public"/> <see cref="CronExpression"/> CronSchedule => <see cref="CronExpression"/>.Parse("* * * * *");
    ///         </c>
    ///     </para>
    ///     </example>
    /// <seealso href="https://crontab.guru/"/>
    /// </remarks>
    CronExpression CronSchedule { get; }


    /// <summary>
    /// The time zone to use for calculating the occurrences of <see cref="CronExpression"/>s.
    /// </summary>
    /// <value>
    /// The time zone information.
    /// </value>
    TimeZoneInfo TimeZoneInfo { get; }

    /// <summary>
    /// Gets or sets the service lifetime of this 
    /// <see cref="ICronJob"/>.
    /// </summary>
    /// <value>
    /// The service lifetime.
    /// </value>
    ServiceLifetime ServiceLifetime { get; }
}
