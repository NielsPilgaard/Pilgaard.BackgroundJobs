using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pilgaard.CronJobs.Extensions.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleCronJobs(
        this IServiceCollection services, params Type[] types) =>
        services.AddSimpleCronJobs(types.Select(e => e.Assembly), null);
    public static IServiceCollection AddSimpleCronJobs(
        this IServiceCollection services, params Assembly[] assembliesToScan) =>
        services.AddSimpleCronJobs(assembliesToScan, null);
    public static IServiceCollection AddSimpleCronJobs(
        this IServiceCollection services, Action<CronBackgroundServiceOptions> configuration, params Type[] types) =>
        services.AddSimpleCronJobs(types.Select(e => e.Assembly), configuration);
    public static IServiceCollection AddSimpleCronJobs(
        this IServiceCollection services, Action<CronBackgroundServiceOptions> configuration, params Assembly[] assembliesToScan) =>
        services.AddSimpleCronJobs(assembliesToScan, configuration);

    public static IServiceCollection AddSimpleCronJobs(
        this IServiceCollection services, IEnumerable<Assembly> assembliesToScan, Action<CronBackgroundServiceOptions>? configuration)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for Cron Services.");
        }

        var cronBackgroundServiceOptions = new CronBackgroundServiceOptions();
        configuration?.Invoke(cronBackgroundServiceOptions);

        var typesToMatch = new[] { typeof(ICronService) };

        foreach (var assembly in assembliesToScan)
        {
            var classes = assembly.ExportedTypes.Where(t => !t.IsAbstract && t.GetInterfaces().Any());
            foreach (var @class in classes)
            {
                foreach (var @interface in @class.GetInterfaces().Where(e => e.IsGenericType))
                {
                    foreach (var typeToMatch in typesToMatch)
                    {
                        if (@interface.GetGenericTypeDefinition() == typeToMatch)
                        {
                            services.Add(new ServiceDescriptor(typeToMatch.MakeGenericType(@interface.GetGenericArguments()), @class, cronBackgroundServiceOptions.ServiceLifetime));
                        }
                    }
                }
            }
        }

        return services;
    }
    public static IServiceCollection AddCronBackgroundService<TCronService>(
        this IServiceCollection services,
        Action<CronBackgroundServiceOptions>? configure = null)
        where TCronService : ICronService
    {
        return services.AddHostedService(serviceProvider =>
            new CronBackgroundService<TCronService>(
                serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                configure));
    }
}