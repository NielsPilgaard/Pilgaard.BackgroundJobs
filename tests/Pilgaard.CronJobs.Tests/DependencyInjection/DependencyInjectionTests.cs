using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.CronJobs.Extensions;

namespace Pilgaard.CronJobs.Tests.DependencyInjection;

public class cronjob_registration_should
{
	[Fact]
	public async Task add_cronjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddCronJobs(typeof(cronjob_registration_should));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var registeredCronJob = serviceProvider.GetRequiredService<TestCronJob>();

		registeredCronJob.Should().NotBeNull().And.BeOfType<TestCronJob>().And.BeAssignableTo<ICronJob>();
	}

	[Fact]
	public async Task register_cronjob_as_icronjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddCronJobs(typeof(cronjob_registration_should));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var registeredCronJob = serviceProvider.GetRequiredService<ICronJob>();

		registeredCronJob.Should().NotBeNull().And.BeOfType<TestCronJob>().And.BeAssignableTo<ICronJob>();
	}

	[Fact]
	public async Task not_add_internal_cronjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddCronJobs(typeof(cronjob_registration_should));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var internalCronJob = serviceProvider.GetService(typeof(InternalCronJob));

		internalCronJob.Should().BeNull();
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

internal class InternalCronJob : ICronJob
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
