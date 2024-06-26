using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs.Tests;
public class BackgroundJobServiceTests
{
	private readonly IServiceCollection _services = new ServiceCollection().AddLogging();

	[Fact]
	public async Task run_background_jobs()
	{
		// Arrange
		string? output1 = null;
		string? output2 = null;
		string? output3 = null;
		string? output4 = null;

		_services
			.AddBackgroundJobs()
			.AddJob("FastRecurringJob", () => output1 = "not empty", TimeSpan.FromSeconds(1))
			.AddJob("FastRecurringJobWithInitialDelay", () => output2 = "not empty", TimeSpan.FromSeconds(1), TimeSpan.Zero)
			.AddJob("FastCronJob", () => output3 = "not empty", CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds))
			.AddJob("FastOneTimeJob", () => output4 = "not empty", DateTime.UtcNow.AddSeconds(1));

		await using var serviceProvider = _services.BuildServiceProvider();

		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

		var sut = serviceProvider.GetRequiredService<IBackgroundJobService>();

		// Act
		try
		{
			await sut.RunJobsAsync(cts.Token);
		}
		catch
		{
			// Intentional throw to stop RunJobsAsync
		}

		// Assert
		output1.Should().NotBeNull();
		output2.Should().NotBeNull();
		output3.Should().NotBeNull();
		output4.Should().NotBeNull();
	}

	[Fact]
	public async Task return_early_when_only_recurring_jobs_are_registered()
	{
		// Arrange
		_services.AddBackgroundJobs()
			.AddJob("FastRecurringJob", () => { }, TimeSpan.FromSeconds(1))
			.AddJob("FastRecurringJobWithInitialDelay", () => { }, TimeSpan.FromSeconds(1), TimeSpan.Zero);

		await using var serviceProvider = _services.BuildServiceProvider();

		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

		var sut = serviceProvider.GetRequiredService<IBackgroundJobService>();

		// Act
		await sut.RunJobsAsync(cts.Token);

		// The assertion is that RunJobsAsync does not keep running, because it returns early
	}
}
