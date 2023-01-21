using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pilgaard.BackgroundJobs.Extensions;

namespace Pilgaard.BackgroundJobs;

internal sealed class BackgroundJobScheduler : IBackgroundJobScheduler
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<BackgroundJobServiceOptions> _options;
    private readonly ILogger<BackgroundJobScheduler> _logger;

    public BackgroundJobScheduler(IServiceScopeFactory scopeFactory,
        IOptions<BackgroundJobServiceOptions> options,
        ILogger<BackgroundJobScheduler> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ValidateRegistrations(_options.Value.Registrations);
    }

    public async IAsyncEnumerable<BackgroundJobRegistration> GetBackgroundJobsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var interval = TimeSpan.FromHours(1);

        var backgroundJobOccurrences = GetOrderedBackgroundJobOccurrences(interval);

        if (!backgroundJobOccurrences.Any())
        {
            var intervalMinus5Seconds = interval.Subtract(TimeSpan.FromSeconds(5));

            _logger.LogDebug("No background job occurrences found in the TimeSpan {interval}, " +
                             "waiting for TimeSpan {interval} until checking again.", interval, intervalMinus5Seconds);

            await Task.Delay(intervalMinus5Seconds, cancellationToken);

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

    internal IEnumerable<BackgroundJobOccurrence> GetOrderedBackgroundJobOccurrences(TimeSpan fetchInterval)
    {
        var toUtc = DateTime.UtcNow.Add(fetchInterval);

        using var scope = _scopeFactory.CreateScope();

        var backgroundJobOccurrences = new List<BackgroundJobOccurrence>();
        foreach (var registration in _options.Value.Registrations)
        {
            var backgroundJob = registration.Factory(scope.ServiceProvider);

            backgroundJobOccurrences.AddRange(
                backgroundJob
                    .GetOccurrences(toUtc)
                    .OrderBy(dateTime => dateTime)
                    .Select(occurrence =>
                        new BackgroundJobOccurrence(occurrence, registration)));
        }

        var orderedBackgroundJobOccurrences = backgroundJobOccurrences
            .OrderBy(backgroundJobOccurrence => backgroundJobOccurrence.Occurrence);

        foreach (var backgroundJobOccurrence in orderedBackgroundJobOccurrences)
        {
            yield return backgroundJobOccurrence;
        }
    }

    /// <summary>
    /// Scan the <see cref="ICollection{BackgroundJobRegistration}"/> for duplicate names
    /// to provide an error if there are duplicates.
    /// </summary>
    /// <param name="registrations">The background job registrations.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ValidateRegistrations(ICollection<BackgroundJobRegistration> registrations)
    {
        StringBuilder? builder = null;
        var distinctRegistrations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var registration in registrations)
        {
            if (!distinctRegistrations.Add(registration.Name))
            {
                builder ??= new StringBuilder("Duplicate health checks were registered with the name(s): ");

                builder.Append(registration.Name).Append(", ");
            }
        }

        if (builder is not null)
        {
            throw new ArgumentException(builder.ToString(0, builder.Length - 2), nameof(registrations));
        }
    }
}
