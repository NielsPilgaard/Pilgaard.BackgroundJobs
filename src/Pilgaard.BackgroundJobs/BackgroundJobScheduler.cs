using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pilgaard.BackgroundJobs;

internal sealed class BackgroundJobScheduler : IBackgroundJobScheduler
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<BackgroundJobServiceOptions> _options;
    private readonly ILogger<BackgroundJobScheduler> _logger;

    public BackgroundJobScheduler(IServiceScopeFactory scopeFactory,
        IOptions<BackgroundJobServiceOptions> options,
        IRegistrationValidator registrationsValidator,
        ILogger<BackgroundJobScheduler> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        registrationsValidator.Validate(_options.Value.Registrations);
    }

    public async IAsyncEnumerable<BackgroundJobRegistration> GetBackgroundJobsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var interval = TimeSpan.FromMinutes(1);

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
}

internal readonly record struct BackgroundJobOccurrence(
    DateTime Occurrence,
    BackgroundJobRegistration BackgroundJobRegistration)
{
    public DateTime Occurrence { get; } = Occurrence;
    public BackgroundJobRegistration BackgroundJobRegistration { get; } = BackgroundJobRegistration;
}
