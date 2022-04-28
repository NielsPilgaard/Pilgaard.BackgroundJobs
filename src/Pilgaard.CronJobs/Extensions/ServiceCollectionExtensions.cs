using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pilgaard.CronJobs.Configuration;

namespace Pilgaard.CronJobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronJobs(this IServiceCollection services, params Type[] types)
    {
        return services.AddCronJobs(types.Select(type => type.GetTypeInfo().Assembly), null);
    }

    public static IServiceCollection AddCronJobs(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        return services.AddCronJobs(assembliesToScan, null);
    }

    public static IServiceCollection AddCronJobs(this IServiceCollection services, Action<CronJobOptions> configuration, params Type[] types)
    {
        return services.AddCronJobs(types.Select(type => type.GetTypeInfo().Assembly), configuration);
    }

    public static IServiceCollection AddCronJobs(this IServiceCollection services, Action<CronJobOptions> configuration, params Assembly[] assembliesToScan)
    {
        return services.AddCronJobs(assembliesToScan, configuration);
    }

    public static IServiceCollection AddCronJobs(this IServiceCollection services, IEnumerable<Assembly> assembliesToScan, Action<CronJobOptions>? configuration)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for Cron Services.");
        }

        var cronJobOptions = new CronJobOptions();

        configuration?.Invoke(cronJobOptions);

        foreach (var assembly in assembliesToScan)
        {
            var implementsICronJob = assembly.ExportedTypes.Where(type =>
                !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(ICronJob)));

            foreach (var cronJob in implementsICronJob)
            {
                RegisterCronJob(services, cronJobOptions, cronJob);
                AddHostedCronBackgroundService(services, cronJob, configuration);
            }
        }

        return services;
    }

    private static void RegisterCronJob(IServiceCollection services,
        CronJobOptions cronJobOptions,
        Type concreteClass)
    {
        services.Add(new ServiceDescriptor(
            typeof(ICronJob),
            concreteClass,
            cronJobOptions.ServiceLifetime));

        services.Add(new ServiceDescriptor(
            concreteClass,
            concreteClass,
            cronJobOptions.ServiceLifetime));
    }

    private static void AddHostedCronBackgroundService(
        IServiceCollection services,
        Type @class,
        Action<CronJobOptions>? configuration)
    {
        services.AddSingleton<IHostedService>(serviceProvider =>
            new CronBackgroundService((ICronJob)serviceProvider.GetRequiredService(@class),
                serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                serviceProvider.GetRequiredService<ILogger<CronBackgroundService>>(),
                configuration));
    }
}