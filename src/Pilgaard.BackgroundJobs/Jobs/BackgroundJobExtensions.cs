namespace Pilgaard.BackgroundJobs;

public static class BackgroundJobExtensions
{
	/// <summary>
	/// Get the next occurrence of the background job.
	/// </summary>
	/// <param name="backgroundJob">The background job to get the next occurrence of.</param>
	/// <returns><see cref="DateTime"/> of the next occurrence, or <c>null</c> if none.</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static DateTime? GetNextOccurrence(this IBackgroundJob backgroundJob) =>
		backgroundJob switch
		{
			ICronJob cronJob => cronJob.GetNextOccurrence(),
			IRecurringJob recurringJob => recurringJob.GetNextOccurrence(),
			IOneTimeJob oneTimeJob => oneTimeJob.GetNextOccurrence(),
			_ => throw new ArgumentOutOfRangeException(nameof(backgroundJob),
				$"Background job must implement either {nameof(ICronJob)}, {nameof(IRecurringJob)} or {nameof(IOneTimeJob)}")
		};

	/// <summary>
	/// Get all occurrences of the background job up to a certain date.
	/// </summary>
	/// <param name="backgroundJob">The background job to get the occurrences of.</param>
	/// <param name="toUtc">The date up to which to get occurrences.</param>
	/// <returns><see cref="IEnumerable{T}"/> of all occurrences.</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static IEnumerable<DateTime> GetOccurrences(this IBackgroundJob backgroundJob, DateTime toUtc) =>
		backgroundJob switch
		{
			ICronJob cronJob => cronJob.GetOccurrences(toUtc),
			IRecurringJob recurringJob => recurringJob.GetOccurrences(toUtc),
			IOneTimeJob oneTimeJob => oneTimeJob.GetOccurrences(toUtc),
			_ => throw new ArgumentOutOfRangeException(nameof(backgroundJob),
				$"Background job must implement either {nameof(ICronJob)}, {nameof(IRecurringJob)} or {nameof(IOneTimeJob)}")
		};
}
