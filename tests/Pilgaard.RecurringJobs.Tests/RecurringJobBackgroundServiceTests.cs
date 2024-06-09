using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pilgaard.RecurringJobs.Extensions;

namespace Pilgaard.RecurringJobs.Tests;

public class RecurringJobBackgroundServiceTests : IAsyncLifetime
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly ILogger<RecurringJobBackgroundService> _logger;
	private readonly TestRecurringJob _recurringJob;
	private readonly ServiceProvider _serviceProvider;

	private CancellationTokenSource? _cts;
	private RecurringJobBackgroundService? _sut;

	public RecurringJobBackgroundServiceTests()
	{
		var services = new ServiceCollection()
			.AddRecurringJobs(typeof(RecurringJobBackgroundServiceTests));

		_serviceProvider = services.BuildServiceProvider();
		_recurringJob = _serviceProvider.GetRequiredService<TestRecurringJob>();
		_serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
		_logger = new NullLogger<RecurringJobBackgroundService>();
	}

	[Fact]
	public async Task execute_its_job_when_it_triggers()
	{
		// Arrange

		// Act
		await Task.Delay(TimeSpan.FromSeconds(3));

		// Assert
		_recurringJob.PersistentField.Should().BeGreaterThanOrEqualTo(1);
	}

	[Fact]
	public async Task execute_its_job_the_correct_number_of_times()
	{
		// Arrange

		// Act
		await Task.Delay(TimeSpan.FromSeconds(5));

		// Assert - ExecuteAsync has been received at least 5 times, after 5 seconds.
		_recurringJob.PersistentField.Should().BeGreaterThanOrEqualTo(5);
	}

	[Fact]
	public async Task not_execute_its_job_more_often_than_its_schedule()
	{
		// Arrange

		// Act
		await Task.Delay(TimeSpan.FromSeconds(2));

		// Assert
		_recurringJob.PersistentField.Should().BeInRange(1, 5);
	}

	public async Task InitializeAsync()
	{
		_cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
		_sut = new RecurringJobBackgroundService(_recurringJob, _serviceScopeFactory, _logger);
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

public class TestRecurringJob : IRecurringJob
{
	public int PersistentField { get; set; }

	public Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		PersistentField++;
		return Task.CompletedTask;
	}

	public TimeSpan Interval => TimeSpan.FromSeconds(1);
	public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
	public TimeSpan InitialDelay => TimeSpan.Zero;
}
