using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.RecurringJobs.Extensions;

namespace Pilgaard.RecurringJobs.Tests.DependencyInjection;

public class DependencyInjectionTests
{
	[Fact]
	public async Task add_recurringjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddRecurringJobs(typeof(DependencyInjectionTests));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var registeredJob = serviceProvider.GetRequiredService<TestRecurringJob>();

		registeredJob.Should().NotBeNull().And.BeOfType<TestRecurringJob>().And.BeAssignableTo<IRecurringJob>();
	}

	[Fact]
	public async Task register_recurringjob_as_irecurringjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddRecurringJobs(typeof(DependencyInjectionTests));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var registeredJob = serviceProvider.GetRequiredService<IRecurringJob>();

		registeredJob.Should().NotBeNull().And.BeOfType<TestRecurringJob>().And.BeAssignableTo<IRecurringJob>();
	}

	[Fact]
	public async Task not_add_internal_recurringjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddRecurringJobs(typeof(DependencyInjectionTests));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var internalRecurringJob = serviceProvider.GetService(typeof(InternalCronJob));

		internalRecurringJob.Should().BeNull();
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

internal class InternalCronJob : IRecurringJob
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
