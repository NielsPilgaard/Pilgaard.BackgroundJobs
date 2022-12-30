using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Pilgaard.RecurringJobs.Extensions;
using Xunit;

namespace Pilgaard.RecurringJobs.Tests.DependencyInjection;

public class recurringjob_registration_should
{
    [Fact]
    public async Task add_cronjob_when_properly_configured()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRecurringJobs(typeof(recurringjob_registration_should));

        // Assert
        await using var serviceProvider = services.BuildServiceProvider();

        var registeredCronJob = serviceProvider.GetRequiredService<TestCronJob>();

        registeredCronJob.Should().NotBeNull().And.BeOfType<TestCronJob>().And.BeAssignableTo<IRecurringJob>();
    }

    [Fact]
    public async Task register_cronjob_as_icronjob_when_properly_configured()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRecurringJobs(typeof(recurringjob_registration_should));

        // Assert
        await using var serviceProvider = services.BuildServiceProvider();

        var registeredCronJob = serviceProvider.GetRequiredService<IRecurringJob>();

        registeredCronJob.Should().NotBeNull().And.BeOfType<TestCronJob>().And.BeAssignableTo<IRecurringJob>();
    }

    [Fact]
    public async Task not_add_internal_cronjob_when_properly_configured()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRecurringJobs(typeof(recurringjob_registration_should));

        // Assert
        await using var serviceProvider = services.BuildServiceProvider();

        object? internalCronJob = serviceProvider.GetService(typeof(InternalCronJob));

        internalCronJob.Should().BeNull();
    }
}

public class TestCronJob : IRecurringJob
{
    public int PersistentField { get; set; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        PersistentField++;
        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromSeconds(1);
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
    public TimeSpan InitialDelay => TimeSpan.Zero;
}

internal class InternalCronJob : IRecurringJob
{
    public int PersistentField { get; set; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        PersistentField++;
        return Task.CompletedTask;
    }

    public TimeSpan Interval => TimeSpan.FromSeconds(1);
    public ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
    public TimeSpan InitialDelay => TimeSpan.Zero;
}
