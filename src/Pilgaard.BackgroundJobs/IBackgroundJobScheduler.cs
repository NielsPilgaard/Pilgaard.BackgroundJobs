namespace Pilgaard.BackgroundJobs;

/// <summary>
/// IBackgroundJobScheduler is responsible for scheduling background jobs.
/// </summary>
internal interface IBackgroundJobScheduler
{
	/// <summary>
	/// Asynchronously retrieves an ordered enumerable of background job registrations.
	/// <para>
	/// Jobs that implement <see cref="IRecurringJob"/> are not retrieved, they are scheduled during startup.
	/// </para>
	/// <para>
	/// Each background job registration is returned when it should be run.
	/// </para>
	/// </summary>
	/// <param name="cancellationToken">The <see cref="CancellationToken"/> used for cancelling the enumeration.</param>
	/// <returns>An asynchronous enumerable of background job registrations.</returns>
	IAsyncEnumerable<BackgroundJobRegistration> GetBackgroundJobsAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Retrieves all <see cref="BackgroundJobRegistration"/>s where <see cref="BackgroundJobRegistration.IsRecurringJob"/> is <c>true</c>
	/// </summary>
	/// <returns>An enumerable of background job registrations where <see cref="BackgroundJobRegistration.IsRecurringJob"/> is <c>true</c></returns>
	IEnumerable<BackgroundJobRegistration> GetRecurringJobs();
}
