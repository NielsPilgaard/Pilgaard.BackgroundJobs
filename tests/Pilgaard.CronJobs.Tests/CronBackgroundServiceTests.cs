using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pilgaard.CronJobs.Extensions;

namespace Pilgaard.CronJobs.Tests;

public class cronbackgroundservice_should : IAsyncLifetime
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CronBackgroundService> _logger;
    private readonly TestCronJob _cronJob;
    private readonly TransientCronJob _transientCronJob;
    private readonly ServiceProvider _serviceProvider;

    private CancellationTokenSource? _cts;
    private CronBackgroundService? _sut;

    public cronbackgroundservice_should()
    {
        var services = new ServiceCollection()
            .AddCronJobs(typeof(cronbackgroundservice_should));

        _serviceProvider = services.BuildServiceProvider();
        _cronJob = _serviceProvider.GetRequiredService<TestCronJob>();
        _transientCronJob = _serviceProvider.GetRequiredService<TransientCronJob>();
        _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        _logger = new NullLogger<CronBackgroundService>();
    }

    [Fact]
    public async Task execute_its_cronjob_when_it_triggers()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Assert
        _cronJob.PersistentField.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task execute_its_cronjob_the_correct_number_of_times()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(5));

        // Assert - ExecuteAsync has been received at least 5 times, after 5 seconds.
        _cronJob.PersistentField.Should().BeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task not_execute_its_cronjob_more_often_than_its_schedule()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Assert
        _cronJob.PersistentField.Should().BeInRange(1, 5);
    }

    [Fact]
    public async Task use_transient_cronjob_when_configured_to()
    {
        // Arrange
        var sut = new CronBackgroundService(_transientCronJob, _serviceScopeFactory, _logger);
        await sut.StartAsync(_cts!.Token);

        // Act

        // Assert
        _transientCronJob.PersistentField.Should().Be(0);
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        _transientCronJob.PersistentField.Should().Be(0);
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        _transientCronJob.PersistentField.Should().Be(0);
    }

    [Fact]
    public async Task use_singleton_cronjob_when_configured_to()
    {
        // Arrange

        // Act
        // _sut is configured to use a Singleton CronJob, and that is already running.

        // Assert
        // Assert that the CronJob executes every second
        for (int i = 0; i < 5; i++)
        {
            _cronJob.PersistentField.Should().Be(i);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    public async Task InitializeAsync()
    {
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        _sut = new CronBackgroundService(_cronJob, _serviceScopeFactory, _logger);
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
    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}

public class TransientCronJob : ICronJob
{
    public int PersistentField { get; set; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        PersistentField++;
        return Task.CompletedTask;
    }

    public CronExpression CronSchedule => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
    public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Local;
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
}
