using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.CronJobs.Extensions;

namespace Pilgaard.CronJobs.Benchmarks;

[MemoryDiagnoser]
public class RegistrationBenchmarks
{
    private readonly ServiceCollection _serviceCollection = new();
    
    [Benchmark]
    public void AddCronJobs()
    {
        _serviceCollection.AddCronJobs(typeof(RegistrationBenchmarks));
    }
}