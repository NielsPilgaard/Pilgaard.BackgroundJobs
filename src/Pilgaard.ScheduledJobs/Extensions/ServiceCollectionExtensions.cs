using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pilgaard.ScheduledJobs.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Finds and adds all <see cref="IScheduledJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IScheduledJob"/> is then hosted in a <see cref="ScheduledJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="types">The types to scan for <see cref="IScheduledJob"/>s through.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for ScheduledJobs.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, params Type[] types)
    {
        return services.AddScheduledJobs(types.Select(type => type.GetTypeInfo().Assembly));
    }

    /// <summary>
    ///     Finds and adds all <see cref="IScheduledJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IScheduledJob"/> is then hosted in a <see cref="ScheduledJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="IScheduledJob"/>s.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for ScheduledJobs.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        return services.AddScheduledJobs(assembliesToScan.AsEnumerable());
    }

    /// <summary>
    ///     Finds and adds all <see cref="IScheduledJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IScheduledJob"/> is then hosted in a <see cref="ScheduledJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="IScheduledJob"/>s.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for ScheduledJobs.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    private static IServiceCollection AddScheduledJobs(this IServiceCollection services,
    IEnumerable<Assembly> assembliesToScan)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for ScheduledJobs.");
        }

        foreach (var assembly in assembliesToScan)
        {
            var typesThatImplementInterface = assembly.ExportedTypes.Where(type =>
                !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(IScheduledJob)));

            foreach (var job in typesThatImplementInterface)
            {
                RegisterJob(services, job);
                AddHostedService(services, job);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers the <see cref="IScheduledJob"/> through a <see cref="ServiceDescriptor"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="concreteClass">The concrete class.</param>
    private static void RegisterJob(IServiceCollection services,
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type concreteClass
#endif
#if NETSTANDARD2_0
        Type concreteClass
#endif
    )
    {
        services.Add(new ServiceDescriptor(
            typeof(IScheduledJob),
            concreteClass,
            ServiceLifetime.Transient));

        services.Add(new ServiceDescriptor(
            concreteClass,
            concreteClass,
            ServiceLifetime.Transient));
    }

    /// <summary>
    /// Adds the scheduled job in a hosted background service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="class">The concrete <see cref="IScheduledJob"/> to host.</param>
    private static void AddHostedService(
        IServiceCollection services,
        Type @class)
    {
        services.AddSingleton<IHostedService>(provider =>
            new ScheduledJobBackgroundService((IScheduledJob)provider.GetRequiredService(@class),
                provider.GetRequiredService<IServiceScopeFactory>(),
                provider.GetRequiredService<ILogger<ScheduledJobBackgroundService>>()));
    }
}
