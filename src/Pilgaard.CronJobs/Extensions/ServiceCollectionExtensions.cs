using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgaard.CronJobs.Configuration;

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
    public static IServiceCollection AddCronJobs(this IServiceCollection services, params Type[] types)
    {
        return services.AddCronJobs(types.Select(type => type.GetTypeInfo().Assembly), null);
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
    public static IServiceCollection AddCronJobs(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        return services.AddCronJobs(assembliesToScan, null);
    }

    /// <summary>
    ///     Finds and adds all <see cref="ICronJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="ICronJob"/> is then hosted in a <see cref="CronBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="types">The types to scan for <see cref="ICronJob"/>s through.</param>
    /// <param name="configuration">The configurator of <see cref="CronJobOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
    public static IServiceCollection AddCronJobs(this IServiceCollection services, Action<CronJobOptions>? configuration = null, params Type[] types)
    {
        return services.AddCronJobs(types.Select(type => type.GetTypeInfo().Assembly), configuration);
    }

    /// <summary>
    ///     Finds and adds all <see cref="ICronJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="ICronJob"/> is then hosted in a <see cref="CronBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="ICronJob"/>s.</param>
    /// <param name="configuration">The configurator of <see cref="CronJobOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
    public static IServiceCollection AddCronJobs(this IServiceCollection services, Action<CronJobOptions>? configuration = null, params Assembly[] assembliesToScan)
    {
        return services.AddCronJobs(assembliesToScan, configuration);
    }

    /// <summary>
    ///     Finds and adds all <see cref="ICronJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="ICronJob"/> is then hosted in a <see cref="CronBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="ICronJob"/>s.</param>
    /// <param name="configurationAction">The configurator of <see cref="CronJobOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for Cron Services.</exception>
    private static IServiceCollection AddCronJobs(this IServiceCollection services,
        IEnumerable<Assembly> assembliesToScan,
        Action<CronJobOptions>? configurationAction)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for Cron Services.");
        }

        var cronJobOptions = new CronJobOptions();

        configurationAction?.Invoke(cronJobOptions);

        services.TryAddSingleton(cronJobOptions);

        foreach (var assembly in assembliesToScan)
        {
            var implementsICronJob = assembly.ExportedTypes.Where(type =>
                !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(ICronJob)));

            foreach (var cronJob in implementsICronJob)
            {
                RegisterCronJob(services, cronJobOptions.ServiceLifetime, cronJob);
                AddHostedCronBackgroundService(services, cronJob);
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
    private static void RegisterCronJob(IServiceCollection services,
        ServiceLifetime serviceLifetime,
        Type concreteClass)
    {
        services.Add(new ServiceDescriptor(
            typeof(ICronJob),
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
    /// <param name="class">The concrete <see cref="ICronJob"/> to host.</param>
    private static void AddHostedCronBackgroundService(
        IServiceCollection services,
        Type @class)
    {
        services.AddSingleton<IHostedService>(provider =>
            new CronBackgroundService((ICronJob)provider.GetRequiredService(@class),
                provider.GetRequiredService<IServiceScopeFactory>(),
                provider.GetRequiredService<ILogger<CronBackgroundService>>(),
                provider.GetRequiredService<CronJobOptions>()));
    }
}