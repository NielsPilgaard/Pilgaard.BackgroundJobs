using Microsoft.Extensions.Hosting;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// <see cref="BackgroundService"/> responsible for executing background jobs using the <see cref="IBackgroundJobService"/>.
/// </summary>
internal sealed class BackgroundJobHostedService : BackgroundService
{
    private readonly IBackgroundJobService _backgroundJobService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundJobHostedService"/> class
    /// </summary>
    /// <param name="backgroundJobService">The background job service used to run background jobs.</param>
    public BackgroundJobHostedService(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService ?? throw new ArgumentNullException(nameof(backgroundJobService));
    }

    /// <summary>
    /// Simply calls <see cref="IBackgroundJobService.RunJobsAsync"/>.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await _backgroundJobService.RunJobsAsync(stoppingToken);
}
