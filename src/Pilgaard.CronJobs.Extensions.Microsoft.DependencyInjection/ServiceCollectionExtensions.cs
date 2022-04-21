using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pilgaard.CronJobs.Extensions.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronJobs(
        this IServiceCollection services, params Type[] types) =>
        services.AddCronJobs(types.Select(e => e.Assembly), null);
    public static IServiceCollection AddCronJobs(
        this IServiceCollection services, params Assembly[] assembliesToScan) =>
        services.AddCronJobs(assembliesToScan, null);
    public static IServiceCollection AddCronJobs(
        this IServiceCollection services, Action<CronJobOptions> configuration, params Type[] types) =>
        services.AddCronJobs(types.Select(e => e.Assembly), configuration);
    public static IServiceCollection AddCronJobs(
        this IServiceCollection services, Action<CronJobOptions> configuration, params Assembly[] assembliesToScan) =>
        services.AddCronJobs(assembliesToScan, configuration);

    public static IServiceCollection AddCronJobs(
        this IServiceCollection services, IEnumerable<Assembly> assembliesToScan, Action<CronJobOptions>? configuration)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for Cron Services.");
        }

        var cronJobOptions = new CronJobOptions();
        configuration?.Invoke(cronJobOptions);

        var typesToMatch = new[] { typeof(ICronJob) };

        foreach (var assembly in assembliesToScan)
        {
            var classes = assembly.ExportedTypes.Where(type => !type.IsAbstract && type.GetInterfaces().Any());
            foreach (var @class in classes)
            {
                foreach (var @interface in @class.GetInterfaces())
                {
                    foreach (var typeToMatch in typesToMatch)
                    {
                        if (@interface != typeToMatch)
                        {
                            continue;
                        }

                        services.Add(new ServiceDescriptor(
                            typeToMatch,
                            @class,
                            cronJobOptions.ServiceLifetime));

                        services.Add(new ServiceDescriptor(
                            @class,
                            @class,
                            cronJobOptions.ServiceLifetime));

                        services.AddHostedService(serviceProvider =>
                            new CronBackgroundService((ICronJob)serviceProvider.GetRequiredService(@class),
                                serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                                serviceProvider.GetRequiredService<ILogger<CronBackgroundService>>(),
                                configuration));
                    }
                }
            }
        }

        return services;
    }
}