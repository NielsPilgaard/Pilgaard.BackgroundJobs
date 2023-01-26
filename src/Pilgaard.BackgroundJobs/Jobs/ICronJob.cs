using Cronos;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// This interface represents a background job that runs according to a Cron schedule.
/// </summary>
public interface ICronJob : IBackgroundJob
{
    /// <summary>
    /// Defines when <see cref="IBackgroundJob.RunJobAsync"/> should trigger.
    /// <para>
    /// We recommend that you use a cron expression editor to make your cron expression string.
    /// </para>
    /// </summary>
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
    CronExpression CronExpression { get; }
}
