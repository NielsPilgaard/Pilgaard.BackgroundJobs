using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Pilgaard.CronJobs.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Finds and adds all <see cref="ICronJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="ICronJob"/> is then hosted in a <see cref="CronBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="types">The types to scan for <see cref="ICronJob"/>s through.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddCronJobs(this IServiceCollection services, params Type[] types)
    {
        return services.AddCronJobs(types.Select(type => type.GetTypeInfo().Assembly));
    }

    /// <summary>
    ///     Finds and adds all <see cref="ICronJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="ICronJob"/> is then hosted in a <see cref="CronBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="ICronJob"/>s.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddCronJobs(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        return services.AddCronJobs(assembliesToScan.AsEnumerable());
    }

    /// <summary>
    ///     Finds and adds all <see cref="ICronJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="ICronJob"/> is then hosted in a <see cref="CronBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="ICronJob"/>s.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    private static IServiceCollection AddCronJobs(this IServiceCollection services,
    IEnumerable<Assembly> assembliesToScan)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for Cron Services.");
        }

        foreach (var assembly in assembliesToScan)
        {
            var implementsICronJob = assembly.ExportedTypes.Where(type =>
                !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(ICronJob)));

            foreach (var cronJob in implementsICronJob)
            {
                RegisterCronJob(services, cronJob);
                AddHostedCronBackgroundService(services, cronJob);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers the cron job through a <see cref="ServiceDescriptor"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="concreteClass">The concrete class.</param>
    private static void RegisterCronJob(IServiceCollection services,
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type concreteClass
#endif
#if NETSTANDARD2_0
        Type concreteClass
#endif
    )
    {
        services.Add(new ServiceDescriptor(
            typeof(ICronJob),
            concreteClass,
            ServiceLifetime.Singleton));

        services.Add(new ServiceDescriptor(
            concreteClass,
            concreteClass,
            ServiceLifetime.Singleton));
    }

    /// <summary>
    /// Adds the hosted cron background services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="class">The concrete <see cref="ICronJob"/> to host.</param>
    private static void AddHostedCronBackgroundService(
        IServiceCollection services,
        Type @class)
    {
        services.AddSingleton<IHostedService>(provider =>
            new CronBackgroundService((ICronJob)provider.GetRequiredService(@class),
                provider.GetRequiredService<IServiceScopeFactory>(),
                provider.GetRequiredService<ILogger<CronBackgroundService>>()));
    }
}
