using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.BackgroundJobs.Extensions;
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
            .GetOrderedBackgroundJobOccurrences(DateTime.UtcNow.AddSeconds(30))
            .ToArray();

        // Assert
        var lastOccurrence = DateTime.MinValue;
        foreach (var (occurrence, backgroundJob) in backgroundJobs)
        {
            _testOutput.WriteLine($"[{backgroundJob}]: {occurrence}");
            occurrence.Should().BeAfter(lastOccurrence);
            lastOccurrence = occurrence!.Value;
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

        // Act
        var backgroundJobs = sut.GetBackgroundJobsAsync(DateTime.UtcNow.AddSeconds(5));

        // Assert
        ushort jobCount = 0;
        ushort index = 1;
        await foreach (var backgroundJob in backgroundJobs)
        {
            var now = DateTime.UtcNow;
            now.Second.Should().Be(startTime.AddSeconds(index).Second);
            jobCount++;
            index++;
            _testOutput.WriteLine($"[{backgroundJob}]: {now}");
        }
        jobCount.Should().Be(4);
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
            .GetOrderedBackgroundJobOccurrences(DateTime.UtcNow.AddMinutes(2))
            .ToArray();

        // Assert
        var distinctBackgroundJobs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (occurrence, backgroundJob) in backgroundJobs)
        {
            distinctBackgroundJobs.Add(backgroundJob.GetType().Name);
            _testOutput.WriteLine($"[{backgroundJob}]: {occurrence}");
        }

        distinctBackgroundJobs.Count.Should().Be(3);
    }

    [Fact]
    public async Task throw_an_argument_exception_when_background_jobs_have_the_same_name()
    {
        // Arrange
        _services
            .AddSingleton<BackgroundJobScheduler>()
            .AddBackgroundJobs()
            .AddJob("DuplicateJob", () => { }, TimeSpan.FromSeconds(10))
            .AddJob("DuplicateJob", () => { }, TimeSpan.FromSeconds(10));

        await using var serviceProvider = _services.BuildServiceProvider();

        // Act && Assert
        Assert.Throws<ArgumentException>(() => serviceProvider.GetRequiredService<BackgroundJobScheduler>());
    }
}
