using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pilgaard.BackgroundJobs;

internal sealed class BackgroundJobScheduler
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

    public async IAsyncEnumerable<IBackgroundJob> GetBackgroundJobsAsync(DateTime toUtc, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var backgroundJobOccurrences = GetOrderedBackgroundJobOccurrences(toUtc);

        foreach (var (occurrence, backgroundJob) in backgroundJobOccurrences)
        {
            if (occurrence is null)
            {
                // TODO: Add Logging
                yield break;
            }

            // TODO: Add Logging
            var timeUntilNextOccurrence = occurrence.Value.Subtract(DateTime.UtcNow);
            await Task.Delay(timeUntilNextOccurrence, cancellationToken);
            yield return backgroundJob;
        }
    }

    internal IEnumerable<BackgroundJobOccurrence> GetOrderedBackgroundJobOccurrences(DateTime toUtc)
    {
        using var scope = _scopeFactory.CreateScope();

        var backgroundJobOccurrences = Enumerable.Empty<BackgroundJobOccurrence>();
        foreach (var registration in _options.Value.Registrations)
        {
            var backgroundJob = registration.Factory(scope.ServiceProvider);

            backgroundJobOccurrences = backgroundJob
                .GetOccurrences(toUtc)
                .OrderBy(dateTime => dateTime)
                .Select(occurrence =>
                    new BackgroundJobOccurrence(occurrence, backgroundJob));
        }

        // TODO: Add Logging
        backgroundJobOccurrences = backgroundJobOccurrences
            .OrderBy(backgroundJobOccurrence => backgroundJobOccurrence.Occurrence.GetValueOrDefault());

        foreach (var backgroundJobOccurrence in backgroundJobOccurrences)
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
