using Cronos;

namespace Pilgaard.CronJobs;

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
}