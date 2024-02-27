using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Pilgaard.BackgroundJobs.Tests;
public class backgroundjobscheduler_should
{
    private readonly ITestOutputHelper _testOutput;
    private readonly IServiceCollection _services;

    public backgroundjobscheduler_should(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
        _services = new ServiceCollection().AddLogging();
    }

    [Fact]
    public async Task return_background_jobs_in_order_of_occurrence()
    {
        // Arrange
        _services
            .AddSingleton<BackgroundJobScheduler>()
            .AddBackgroundJobs()
            .AddJob("FastRecurringJob", () => { }, TimeSpan.FromSeconds(10))
            .AddJob("FasterRecurringJob", () => { }, TimeSpan.FromSeconds(3));

        await using var serviceProvider = _services.BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<BackgroundJobScheduler>();

        // Act
        var backgroundJobs = sut
            .GetOrderedBackgroundJobOccurrences(TimeSpan.FromSeconds(30))
            .ToArray();

        // Assert
        var lastOccurrence = DateTime.MinValue;
        foreach (var (occurrence, backgroundJob) in backgroundJobs)
        {
            _testOutput.WriteLine($"[{backgroundJob}]: {occurrence}");
            occurrence.Should().BeAfter(lastOccurrence);
            lastOccurrence = occurrence;
        }
    }

    [Fact(Timeout = 5500)]
    public async Task return_background_jobs_when_they_should_be_run()
    {
        // Arrange
        _services
            .AddSingleton<BackgroundJobScheduler>()
            .AddBackgroundJobs()
            .AddJob("RecurringJob", () => { }, TimeSpan.FromSeconds(1));

        await using var serviceProvider = _services.BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<IBackgroundJobScheduler>();

        var startTime = DateTime.UtcNow;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        // Act
        var backgroundJobs = sut.GetBackgroundJobsAsync(cts.Token);

        // Assert
        ushort index = 1;

        try
        {
            await foreach (var backgroundJob in backgroundJobs)
            {
                var now = DateTime.UtcNow;
                now.Second.Should().Be(startTime.AddSeconds(index++).Second);
                _testOutput.WriteLine($"[{backgroundJob}]: {now}");
            }
        }
        catch
        {
            // This test throws to stop
        }
    }

    [Fact]
    public async Task be_able_to_return_all_types_of_background_job()
    {
        // Arrange
        _services
            .AddSingleton<BackgroundJobScheduler>()
            .AddBackgroundJobs()
            .AddJob("RecurringJob", () => { }, TimeSpan.FromSeconds(10))
            .AddJob("CronJob", () => { }, CronExpression.Parse("0,30 * * * * *", CronFormat.IncludeSeconds))
            .AddJob("OneTimeJob", () => { }, DateTime.UtcNow.AddSeconds(17));

        await using var serviceProvider = _services.BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<BackgroundJobScheduler>();

        // Act
        var backgroundJobs = sut
            .GetOrderedBackgroundJobOccurrences(TimeSpan.FromMinutes(2))
            .ToArray();
        var recurringJobs = sut.GetRecurringJobs();

        // Assert
        var distinctBackgroundJobs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Recurring jobs are returned separately, add it to the list manually
            recurringJobs.FirstOrDefault()!.Name
        };
        foreach (var (occurrence, backgroundJobRegistration) in backgroundJobs)
        {
            string backgroundJobType = backgroundJobRegistration.Factory(serviceProvider).GetType().Name;
            distinctBackgroundJobs.Add(backgroundJobType);
            _testOutput.WriteLine($"[{backgroundJobType}]: {occurrence}");
        }

        distinctBackgroundJobs.Should().HaveCount(3);
    }

    [Fact]
    public async Task throw_an_argument_exception_when_background_jobs_have_the_same_name()
    {
        // Arrange
        _services
            .AddBackgroundJobs()
            .AddJob("DuplicateJob", () => { }, TimeSpan.FromSeconds(10))
            .AddJob("DuplicateJob", () => { }, TimeSpan.FromSeconds(10));

        await using var serviceProvider = _services.BuildServiceProvider();

        // Act && Assert
        Assert.Throws<ArgumentException>(serviceProvider.GetRequiredService<IBackgroundJobScheduler>);
    }

    [Fact]
    public async Task not_return_more_cronjob_occurrences_than_there_are()
    {
        // Arrange
        _services
            .AddSingleton<BackgroundJobScheduler>()
            .AddBackgroundJobs()
            // Run every 5 seconds
            .AddJob("CronJob", () => { }, CronExpression.Parse("*/5 * * * * *", CronFormat.IncludeSeconds));

        await using var serviceProvider = _services.BuildServiceProvider();
        var sut = serviceProvider.GetRequiredService<BackgroundJobScheduler>();

        // Act
        var backgroundJobs = sut
            .GetOrderedBackgroundJobOccurrences(TimeSpan.FromMinutes(1))
            .ToArray();

        // Assert
        backgroundJobs.Length.Should().Be(60 / 5);
    }
}
