using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.CronJobs.Extensions.Microsoft.DependencyInjection;

namespace Pilgaard.CronJobs.Benchmarks;

[MemoryDiagnoser]
public class RegistrationBenchmarks
{
    [Benchmark]
    public void AddCronJobs()
    {
        new ServiceCollection().AddCronJobs(typeof(RegistrationBenchmarks));
    }
}