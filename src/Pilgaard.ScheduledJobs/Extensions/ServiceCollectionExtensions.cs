using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgaard.ScheduledJobs.Configuration;

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
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, params Type[] types)
    {
        return services.AddScheduledJobs(types.Select(type => type.GetTypeInfo().Assembly), null);
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
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        return services.AddScheduledJobs(assembliesToScan, null);
    }

    /// <summary>
    ///     Finds and adds all <see cref="IScheduledJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IScheduledJob"/> is then hosted in a <see cref="ScheduledJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="types">The types to scan for <see cref="IScheduledJob"/>s through.</param>
    /// <param name="configuration">The configurator of <see cref="ScheduledJobOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, Action<ScheduledJobOptions>? configuration = null, params Type[] types)
    {
        return services.AddScheduledJobs(types.Select(type => type.GetTypeInfo().Assembly), configuration);
    }

    /// <summary>
    ///     Finds and adds all <see cref="IScheduledJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IScheduledJob"/> is then hosted in a <see cref="ScheduledJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="IScheduledJob"/>s.</param>
    /// <param name="configuration">The configurator of <see cref="ScheduledJobOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, Action<ScheduledJobOptions>? configuration = null, params Assembly[] assembliesToScan)
    {
        return services.AddScheduledJobs(assembliesToScan, configuration);
    }

    /// <summary>
    ///     Finds and adds all <see cref="IScheduledJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IScheduledJob"/> is then hosted in a <see cref="ScheduledJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="IScheduledJob"/>s.</param>
    /// <param name="configurationAction">The configurator of <see cref="ScheduledJobOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    private static IServiceCollection AddScheduledJobs(this IServiceCollection services,
    IEnumerable<Assembly> assembliesToScan,
        Action<ScheduledJobOptions>? configurationAction)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for Cron Services.");
        }

        var options = new ScheduledJobOptions();

        configurationAction?.Invoke(options);

        services.TryAddSingleton(options);

        foreach (var assembly in assembliesToScan)
        {
            var typesThatImplementInterface = assembly.ExportedTypes.Where(type =>
                !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(IScheduledJob)));

            foreach (var job in typesThatImplementInterface)
            {
                RegisterJob(services, options.ServiceLifetime, job);
                AddHostedService(services, job);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers the cron job through a <see cref="ServiceDescriptor"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceLifetime">The service life time.</param>
    /// <param name="concreteClass">The concrete class.</param>
    private static void RegisterJob(IServiceCollection services,
        ServiceLifetime serviceLifetime,
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
            serviceLifetime));

        services.Add(new ServiceDescriptor(
            concreteClass,
            concreteClass,
            serviceLifetime));
    }

    /// <summary>
    /// Adds the hosted cron background services.
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
                provider.GetRequiredService<ILogger<ScheduledJobBackgroundService>>(),
                provider.GetRequiredService<ScheduledJobOptions>()));
    }
}
