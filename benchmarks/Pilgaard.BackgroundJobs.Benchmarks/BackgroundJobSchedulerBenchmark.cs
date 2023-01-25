using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.BackgroundJobs.Benchmarks;

[MemoryDiagnoser(false)]
public class BackgroundJobSchedulerBenchmark
{
    private static BackgroundJobScheduler? _backgroundServiceScheduler;
    private static ServiceProvider? _provider;

    [Params(1, 10, 100, 1000)]
    public uint RecurringJobIntervalSeconds { get; set; }

    [Params(1, 10, 100, 1000)]
    public uint BackgroundJobSchedulerFetchInterval { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var services = new ServiceCollection();

        services.AddBackgroundJobs()
            .AddJob("recurring-job", () => { }, TimeSpan.FromSeconds(RecurringJobIntervalSeconds));

        _provider = services.BuildServiceProvider();

        _backgroundServiceScheduler = ActivatorUtilities.CreateInstance<BackgroundJobScheduler>(_provider);
    }

    [Benchmark]
    [BenchmarkCategory(nameof(BackgroundJobScheduler))]
    public void GetOrderedBackgroundJobOccurrencesWithoutIteration()
    {
        var orderedBackgroundJobOccurrences = _backgroundServiceScheduler!
            .GetOrderedBackgroundJobOccurrences(TimeSpan.FromMinutes(BackgroundJobSchedulerFetchInterval));
    }

    [Benchmark]
    [BenchmarkCategory(nameof(BackgroundJobScheduler))]
    public void GetOrderedBackgroundJobOccurrencesWithIteration()
    {
        foreach (var orderedBackgroundJobOccurrence in _backgroundServiceScheduler!
                     .GetOrderedBackgroundJobOccurrences(TimeSpan.FromMinutes(BackgroundJobSchedulerFetchInterval)))
        {

        }
    }

    [GlobalCleanup]
    public void GlobalCleanup() => _provider?.Dispose();
}
