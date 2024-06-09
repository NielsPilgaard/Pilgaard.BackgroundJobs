using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Pilgaard.BackgroundJobs.Tests.DependencyInjection;

public class DependencyInjectionTests
{
	[Fact]
	public async Task add_cronjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddBackgroundJobs()
			.AddJob<CronJob>(nameof(CronJob));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations
			.FirstOrDefault().Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeFalse();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<CronJob>()
			.And.BeAssignableTo<ICronJob>()
			.And.BeAssignableTo<IBackgroundJob>();
	}

	[Fact]
	public async Task add_recurringjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddBackgroundJobs()
			.AddJob<RecurringJob>(nameof(RecurringJob));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeTrue();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<RecurringJob>()
			.And.BeAssignableTo<IRecurringJob>()
			.And.BeAssignableTo<IBackgroundJob>();
	}

	[Fact]
	public async Task add_onetimejob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddBackgroundJobs()
			.AddJob<OneTimeJob>(nameof(OneTimeJob));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeFalse();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<OneTimeJob>()
			.And.BeAssignableTo<IOneTimeJob>()
			.And.BeAssignableTo<IBackgroundJob>();
	}
}

public class CronJob : ICronJob
{
	public Task RunJobAsync(CancellationToken cancellationToken = default)
	{
		Console.WriteLine("Time to backup your databases!");

		return Task.CompletedTask;
	}
	public CronExpression CronExpression => CronExpression.Parse("0 3 * * *");
}

public class RecurringJob : IRecurringJob
{
	public Task RunJobAsync(CancellationToken cancellationToken = default)
	{
		Console.WriteLine("This is your hourly reminder to stay hydrated.");

		return Task.CompletedTask;
	}
	public TimeSpan Interval => TimeSpan.FromHours(1);
}

public class OneTimeJob : IOneTimeJob
{
	public Task RunJobAsync(CancellationToken cancellationToken = default)
	{
		Console.WriteLine("Happy New Year!");

		return Task.CompletedTask;
	}
	public DateTime ScheduledTimeUtc => new(year: 2025, month: 12, day: 31, hour: 23, minute: 59, second: 59);
}
