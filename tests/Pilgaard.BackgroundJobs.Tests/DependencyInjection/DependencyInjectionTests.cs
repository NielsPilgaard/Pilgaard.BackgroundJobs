using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pilgaard.BackgroundJobs.Extensions;

namespace Pilgaard.BackgroundJobs.Tests.DependencyInjection;

public class backgroundjob_registration_should
{
    [Fact]
    public async Task add_cronjob_when_properly_configured()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBackgroundJobs()
            .AddJob<TestCronJob>(nameof(TestCronJob));

        // Assert
        await using var serviceProvider = services.BuildServiceProvider();

        var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

        backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
            .Should().NotBeNull();

        backgroundJobServiceOptions.Value.Registrations
            .First().Factory(serviceProvider)
            .Should().BeOfType<TestCronJob>()
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
            .AddJob<TestRecurringJob>(nameof(TestRecurringJob));

        // Assert
        await using var serviceProvider = services.BuildServiceProvider();

        var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

        backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
            .Should().NotBeNull();

        backgroundJobServiceOptions.Value.Registrations
            .First().Factory(serviceProvider)
            .Should().BeOfType<TestRecurringJob>()
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
            .AddJob<TestOneTimeJob>(nameof(TestOneTimeJob));

        // Assert
        await using var serviceProvider = services.BuildServiceProvider();

        var backgroundJobServiceOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobServiceOptions>>();

        backgroundJobServiceOptions.Value.Registrations.FirstOrDefault()
            .Should().NotBeNull();

        backgroundJobServiceOptions.Value.Registrations
            .First().Factory(serviceProvider)
            .Should().BeOfType<TestOneTimeJob>()
            .And.BeAssignableTo<IOneTimeJob>()
            .And.BeAssignableTo<IBackgroundJob>();
    }
}

internal class TestCronJob : ICronJob
{
    public Task RunJobAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public CronExpression CronExpression => CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds);
}

internal class TestRecurringJob : IRecurringJob
{
    public Task RunJobAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public TimeSpan Interval => TimeSpan.FromSeconds(10);
}

internal class TestOneTimeJob : IOneTimeJob
{
    public Task RunJobAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public DateTime ScheduledTimeUtc => DateTime.UtcNow.AddSeconds(10);
}
