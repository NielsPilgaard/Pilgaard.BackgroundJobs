using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// BackgroundJobScheduler is a class responsible for scheduling background jobs.
/// </summary>
internal sealed class BackgroundJobScheduler : IBackgroundJobScheduler
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IOptions<BackgroundJobServiceOptions> _options;
	private readonly ILogger<BackgroundJobScheduler> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="BackgroundJobScheduler"/> class
	/// </summary>
	/// <param name="scopeFactory">The factory used when constructing background jobs.</param>
	/// <param name="options">The options used for accessing background job registrations.</param>
	/// <param name="registrationsValidator">The validator used for validating the background job registrations.</param>
	/// <param name="logger">The logger.</param>
	public BackgroundJobScheduler(IServiceScopeFactory scopeFactory,
		IOptions<BackgroundJobServiceOptions> options,
		IRegistrationValidator registrationsValidator,
		ILogger<BackgroundJobScheduler> logger)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
		_options = options ?? throw new ArgumentNullException(nameof(options));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));

		if (registrationsValidator is null)
		{
			throw new ArgumentNullException(nameof(registrationsValidator));
		}

		registrationsValidator.Validate(_options.Value.Registrations);
	}

	public async IAsyncEnumerable<BackgroundJobRegistration> GetBackgroundJobsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var interval = TimeSpan.FromSeconds(30);

		var backgroundJobOccurrences = GetOrderedBackgroundJobOccurrences(interval);

		// Check if there's anything to enumerate
		// If false, sleep and return
		if (backgroundJobOccurrences.Count <= 0)
		{
			var intervalMinus5Seconds = interval.Subtract(TimeSpan.FromSeconds(5));

			_logger.LogDebug("No CronJob or OneTimeJob occurrences found in the TimeSpan {interval}, " +
							 "waiting for TimeSpan {interval} until checking again.",
							 interval, intervalMinus5Seconds);

			await Task.Delay(intervalMinus5Seconds, cancellationToken);

			// When we yield break, GetBackgroundJobsAsync will be called again immediately
			yield break;
		}

		foreach (var (occurrence, backgroundJob) in backgroundJobOccurrences)
		{
			var timeUntilNextOccurrence = occurrence.Subtract(DateTime.UtcNow);

			_logger.LogDebug("Background job {jobName} will execute in {timeUntilNextOccurrence}",
				backgroundJob.Name, timeUntilNextOccurrence);

			if (timeUntilNextOccurrence > TimeSpan.Zero)
			{
				await Task.Delay(timeUntilNextOccurrence, cancellationToken);
			}

			yield return backgroundJob;
		}
	}

	public IEnumerable<BackgroundJobRegistration> GetRecurringJobs() => _options.Value.Registrations.Where(registration => registration.IsRecurringJob);

	/// <summary>
	/// Gets an ordered enumerable of background job occurrences within the specified <paramref name="fetchInterval"/>.
	/// </summary>
	/// <param name="fetchInterval">The interval to get occurrences for.</param>
	/// <returns></returns>
	internal List<BackgroundJobOccurrence> GetOrderedBackgroundJobOccurrences(TimeSpan fetchInterval)
	{
		var toUtc = DateTime.UtcNow.Add(fetchInterval);

		using var scope = _scopeFactory.CreateScope();

		var backgroundJobOccurrences = new List<BackgroundJobOccurrence>();

		foreach (var registration in _options.Value.Registrations.Where(registration => !registration.IsRecurringJob))
		{
			var backgroundJob = registration.Factory(scope.ServiceProvider);

			backgroundJobOccurrences.AddRange(
				backgroundJob
					.GetOccurrences(toUtc)
					// deduplicate the occurrences
					.Distinct()
					.OrderBy(dateTime => dateTime)
					.Select(occurrence =>
						new BackgroundJobOccurrence(occurrence, registration)));
		}

		return backgroundJobOccurrences
			.OrderBy(x => x.Occurrence)
			.ToList();
	}
}

/// <summary>
/// A background job registration and one of its occurrences.
/// </summary>
/// <param name="Occurrence">The time the background job should run.</param>
/// <param name="BackgroundJobRegistration">The background job registration.</param>
internal readonly record struct BackgroundJobOccurrence(
	DateTime Occurrence,
	BackgroundJobRegistration BackgroundJobRegistration)
{
	public DateTime Occurrence { get; } = Occurrence;
	public BackgroundJobRegistration BackgroundJobRegistration { get; } = BackgroundJobRegistration;
}
