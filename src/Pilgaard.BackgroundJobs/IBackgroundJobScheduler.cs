namespace Pilgaard.BackgroundJobs;


/// <summary>
/// IBackgroundJobScheduler is responsible for scheduling background jobs.
/// </summary>
internal interface IBackgroundJobScheduler
{
    /// <summary>
    /// Asynchronously retrieves an ordered enumerable of background job registrations.
    /// <para>
    /// Each background job registration is returned when it should be run.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used for cancelling the enumeration.</param>
    /// <returns>An asynchronous enumerable of background job registrations.</returns>
    IAsyncEnumerable<BackgroundJobRegistration> GetBackgroundJobsAsync(CancellationToken cancellationToken);
}
