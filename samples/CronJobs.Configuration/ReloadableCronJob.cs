using Cronos;

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

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ReloadableCronJob.CronSchedule: {cronSchedule}", CronSchedule);

        return Task.CompletedTask;
    }

    public CronExpression CronSchedule =>
        CronExpression.Parse(
            _configuration.GetValue<string>("ReloadableCronJob:CronSchedule"),
            _configuration.GetValue<CronFormat>("ReloadableCronJob:CronFormat"));

    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
}
