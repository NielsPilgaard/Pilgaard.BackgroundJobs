using Microsoft.Extensions.Hosting;

namespace Pilgaard.BackgroundJobs;

internal sealed class BackgroundJobHostedService : BackgroundService
{
    private readonly IBackgroundJobService _backgroundJobService;

    public BackgroundJobHostedService(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService ?? throw new ArgumentNullException(nameof(backgroundJobService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await _backgroundJobService.RunJobsAsync(stoppingToken);
}
