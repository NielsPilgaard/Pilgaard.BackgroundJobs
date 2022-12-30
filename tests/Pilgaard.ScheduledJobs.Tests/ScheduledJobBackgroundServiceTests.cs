using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pilgaard.ScheduledJobs.Configuration;
using Pilgaard.ScheduledJobs.Extensions;

namespace Pilgaard.ScheduledJobs.Tests;

public class scheduledjobbackgroundservice_should : IAsyncLifetime
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ScheduledJobBackgroundService> _logger;
    private readonly TestScheduledJob _job;
    private readonly ServiceProvider _serviceProvider;

    private readonly ScheduledJobOptions _options;
    private CancellationTokenSource? _cts;
    private ScheduledJobBackgroundService? _sut;

    public scheduledjobbackgroundservice_should()
    {
        _options = new ScheduledJobOptions();

        var services = new ServiceCollection()
            .AddScheduledJobs(options => options.ServiceLifetime = ServiceLifetime.Singleton,
                typeof(scheduledjobbackgroundservice_should));

        _serviceProvider = services.BuildServiceProvider();
        _job = _serviceProvider.GetRequiredService<TestScheduledJob>();
        _serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        _logger = new NullLogger<ScheduledJobBackgroundService>();
    }

    [Fact]
    public async Task execute_its_job_when_it_triggers()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(6));

        // Assert
        _job.PersistentField.Should().Be(1);
    }

    [Fact]
    public async Task not_execute_its_job_when_it_does_not_trigger()
    {
        // Arrange

        // Act
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Assert
        _job.PersistentField.Should().Be(0);
    }

    public async Task InitializeAsync()
    {
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        _sut = new ScheduledJobBackgroundService(_job, _serviceScopeFactory, _logger, _options);
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

public class TestScheduledJob : IScheduledJob
{
    public int PersistentField { get; set; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        PersistentField++;
        return Task.CompletedTask;
    }

    public DateTime ScheduledTimeUtc => DateTime.UtcNow.AddSeconds(5);
}
