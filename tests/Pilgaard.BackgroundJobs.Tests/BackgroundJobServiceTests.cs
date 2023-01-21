using Cronos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.BackgroundJobs.Extensions;
using Xunit.Abstractions;

namespace Pilgaard.BackgroundJobs.Tests;
public class backgroundjobservice_should
{
    private readonly ITestOutputHelper _testOutput;
    private readonly IServiceCollection _services;

    public backgroundjobservice_should(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
        _services = new ServiceCollection().AddLogging();
    }

    [Fact]
    public async Task run_background_jobs()
    {
        // Arrange
        string? output1 = null;
        string? output2 = null;

        _services
            .AddSingleton<BackgroundJobScheduler>()
            .AddBackgroundJobs()
            .AddJob("FastRecurringJob", () => output1 = "not empty", TimeSpan.FromSeconds(1))
            .AddJob("FasterRecurringJob", () => output2 = "not empty", CronExpression.Parse("* * * * * *", CronFormat.IncludeSeconds));

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
    }
}
