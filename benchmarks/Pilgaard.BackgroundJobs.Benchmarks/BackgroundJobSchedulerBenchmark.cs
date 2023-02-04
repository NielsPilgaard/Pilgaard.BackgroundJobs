using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs.Benchmarks;

[MemoryDiagnoser(false)]
public class BackgroundJobSchedulerBenchmark
{
    private static BackgroundJobScheduler? _backgroundServiceScheduler;
    private static ServiceProvider? _provider;

    [Params(1, 10, 100, 1000)]
    public static uint RecurringJobIntervalSeconds { get; set; }

    [Params(30)]
    public static uint BackgroundJobSchedulerFetchIntervalSeconds { get; set; }

    [GlobalSetup]
    public static void GlobalSetup()
    {
        var services = new ServiceCollection();

        services
            .AddLogging()
            .AddBackgroundJobs()
            .AddJob("recurring-job", () => { }, TimeSpan.FromSeconds(RecurringJobIntervalSeconds));

        _provider = services.BuildServiceProvider();

        _backgroundServiceScheduler = ActivatorUtilities.CreateInstance<BackgroundJobScheduler>(_provider);
    }

    [Benchmark]
    [BenchmarkCategory(nameof(BackgroundJobScheduler))]
    public void GetOrderedBackgroundJobOccurrencesWithIteration()
    {
        foreach (var orderedBackgroundJobOccurrence in _backgroundServiceScheduler!
                     .GetOrderedBackgroundJobOccurrences(TimeSpan.FromSeconds(BackgroundJobSchedulerFetchIntervalSeconds)))
        {

        }
    }

    [GlobalCleanup]
    public static void GlobalCleanup() => _provider?.Dispose();
}
