using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pilgaard.CronJobs.Configuration;
using Pilgaard.CronJobs.Extensions;
using Xunit;

namespace Pilgaard.CronJobs.Tests;

public class CronBackgroundServiceTests : IAsyncLifetime
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CronBackgroundService> _logger;
    private readonly TestCronJob _cronJob;
    private readonly ServiceProvider _serviceProvider;

    private readonly CronJobOptions _options;
    private CancellationTokenSource? _cts;
    private CronBackgroundService? _sut;

    public CronBackgroundServiceTests()
    {
        _options = new CronJobOptions();

        var services = new ServiceCollection()
            .AddCronJobs(options => options.ServiceLifetime = ServiceLifetime.Singleton,
                typeof(CronBackgroundServiceTests));

        _serviceProvider = services.BuildServiceProvider();
        _cronJob = _serviceProvider.GetRequiredService<TestCronJob>();
        _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        _logger = new NullLogger<CronBackgroundService>();
    }

    [Fact]
    public async Task When_CronBackgroundService_IsRunning_ItsCronJob_IsExecuted()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Assert - ExecuteAsync has been received at least once, after 3 seconds.
        _cronJob.PersistentField.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task When_CronBackgroundService_IsRunning_ItsCronJob_IsExecuted_TheRightNumberOfTimes()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(5));

        // Assert - ExecuteAsync has been received at least 5 times, after 5 seconds.
        _cronJob.PersistentField.Should().BeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task When_CronBackgroundService_IsRunning_ItsCronJob_IsNotExecuted_MoreThanItShould()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Assert
        _cronJob.PersistentField.Should().BeInRange(1, 5);
    }

    public async Task InitializeAsync()
    {
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        _sut = new CronBackgroundService(_cronJob, _serviceScopeFactory, _logger, _options);
        await _sut.StartAsync(_cts.Token);
    }

    public async Task DisposeAsync()
    {
        await _sut!.StopAsync(_cts!.Token);
        await _serviceProvider.DisposeAsync();
        _cts.Cancel();
        _cts.Dispose();
    }
}

public class TestCronJob : ICronJob
{
    public int PersistentField { get; set; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        PersistentField++;
        return Task.CompletedTask;
    }

    public CronExpression CronSchedule => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
}
