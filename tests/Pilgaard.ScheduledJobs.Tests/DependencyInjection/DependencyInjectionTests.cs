using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.ScheduledJobs.Extensions;

namespace Pilgaard.ScheduledJobs.Tests.DependencyInjection;

public class DependencyInjectionTests
{
	[Fact]
	public async Task add_scheduledjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddScheduledJobs(typeof(DependencyInjectionTests));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var registeredJob = serviceProvider.GetRequiredService<TestScheduledJob>();

		registeredJob.Should().NotBeNull().And.BeOfType<TestScheduledJob>().And.BeAssignableTo<IScheduledJob>();
	}

	[Fact]
	public async Task register_scheduledjob_as_ischeduledjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddScheduledJobs(typeof(DependencyInjectionTests));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var registeredJob = serviceProvider.GetRequiredService<IScheduledJob>();

		registeredJob.Should().NotBeNull().And.BeOfType<TestScheduledJob>().And.BeAssignableTo<IScheduledJob>();
	}

	[Fact]
	public async Task not_add_internal_scheduledjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddScheduledJobs(typeof(DependencyInjectionTests));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var internalJob = serviceProvider.GetService(typeof(InternalScheduledJob));

		internalJob.Should().BeNull();
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
	public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}

internal class InternalScheduledJob : IScheduledJob
{
	public int PersistentField { get; set; }

	public Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		PersistentField++;
		return Task.CompletedTask;
	}

	public DateTime ScheduledTimeUtc => DateTime.UtcNow.AddSeconds(5);
	public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
}
