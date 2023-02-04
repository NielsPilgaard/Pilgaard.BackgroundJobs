using Cronos;
using Pilgaard.BackgroundJobs;

namespace Pilgaard.CronJobs.Examples.Configuration;

public class ReloadableCronJob : ICronJob
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReloadableCronJob> _logger;

    public ReloadableCronJob(IConfiguration configuration, ILogger<ReloadableCronJob> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task RunJobAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ReloadableCronJob.CronSchedule: {cronSchedule}", CronExpression);

        return Task.CompletedTask;
    }

    public CronExpression CronExpression =>
        CronExpression.Parse(
            _configuration.GetValue<string>("ReloadableCronJob:CronSchedule"),
            _configuration.GetValue<CronFormat>("ReloadableCronJob:CronFormat"));
}
