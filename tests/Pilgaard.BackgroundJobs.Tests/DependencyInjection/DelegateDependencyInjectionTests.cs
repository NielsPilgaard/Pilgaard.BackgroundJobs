using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Pilgaard.BackgroundJobs.Tests.DependencyInjection;

public class DelegateDependencyInjectionTests
{
	[Fact]
	public async Task add_cronjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddBackgroundJobs()
			.AddJob(
				name: nameof(CronJob),
				job: () => Console.WriteLine("Hi!"),
				cronExpression: CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeFalse();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<DelegateCronJob>()
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
			.AddJob(
				name: nameof(RecurringJob),
				job: () => Console.WriteLine("Hi!"),
				interval: TimeSpan.FromSeconds(10));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeTrue();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<DelegateRecurringJob>()
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
			.AddJob(
				name: nameof(OneTimeJob),
				job: () => Console.WriteLine("Hi!"),
				scheduledTimeUtc: DateTime.UtcNow.AddSeconds(10));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeFalse();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<DelegateOneTimeJob>()
			.And.BeAssignableTo<IOneTimeJob>()
			.And.BeAssignableTo<IBackgroundJob>();
	}

	[Fact]
	public async Task add_async_cronjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddBackgroundJobs()
			.AddAsyncJob(
				name: nameof(CronJob),
				job: (_) => Task.CompletedTask,
				cronExpression: CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeFalse();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<DelegateCronJob>()
			.And.BeAssignableTo<ICronJob>()
			.And.BeAssignableTo<IBackgroundJob>();
	}

	[Fact]
	public async Task add_async_recurringjob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddBackgroundJobs()
			.AddAsyncJob(
				name: nameof(RecurringJob),
				job: (_) => Task.CompletedTask,
				interval: TimeSpan.FromSeconds(10));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeTrue();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<DelegateRecurringJob>()
			.And.BeAssignableTo<IRecurringJob>()
			.And.BeAssignableTo<IBackgroundJob>();
	}

	[Fact]
	public async Task add_async_onetimejob_when_properly_configured()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddBackgroundJobs()
			.AddAsyncJob(
				name: nameof(OneTimeJob),
				job: (_) => Task.CompletedTask,
				scheduledTimeUtc: DateTime.UtcNow.AddSeconds(10));

		// Assert
		await using var serviceProvider = services.BuildServiceProvider();

		var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

		backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
			.Should().NotBeNull();

		backgroundJobServiceOptions.Value.Registrations
			.First().IsRecurringJob.Should().BeFalse();

		backgroundJobServiceOptions.Value.Registrations
			.First().Factory(serviceProvider)
			.Should().BeOfType<DelegateOneTimeJob>()
			.And.BeAssignableTo<IOneTimeJob>()
			.And.BeAssignableTo<IBackgroundJob>();
	}
}
