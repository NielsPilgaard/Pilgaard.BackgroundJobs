using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pilgaard.RecurringJobs.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Finds and adds all <see cref="IRecurringJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IRecurringJob"/> is then hosted in a <see cref="RecurringJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="types">The types to scan for <see cref="IRecurringJob"/>s through.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for RecurringJobs.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddRecurringJobs(this IServiceCollection services, params Type[] types)
    {
        return services.AddRecurringJobs(types.Select(type => type.GetTypeInfo().Assembly));
    }

    /// <summary>
    ///     Finds and adds all <see cref="IRecurringJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IRecurringJob"/> is then hosted in a <see cref="RecurringJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="IRecurringJob"/>s.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for RecurringJobs.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    public static IServiceCollection AddRecurringJobs(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        return services.AddRecurringJobs(assembliesToScan.AsEnumerable());
    }

    /// <summary>
    ///     Finds and adds all <see cref="IRecurringJob"/>s to the <see cref="IServiceCollection"/>.
    /// <para>
    ///     Each <see cref="IRecurringJob"/> is then hosted in a <see cref="RecurringJobBackgroundService"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for <see cref="IRecurringJob"/>s.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further chaining.</returns>
    /// <exception cref="ArgumentException">No assemblies found to scan. Supply at least one assembly to scan for RecurringJobs.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.ExportedTypes")]
#endif
    private static IServiceCollection AddRecurringJobs(this IServiceCollection services,
    IEnumerable<Assembly> assembliesToScan)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for RecurringJobs.");
        }

        foreach (var assembly in assembliesToScan)
        {
            var implementsIRecurringJob = assembly.ExportedTypes.Where(type =>
                !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(IRecurringJob)));

            foreach (var job in implementsIRecurringJob)
            {
                RegisterJob(services, job);
                AddHostedService(services, job);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers the job through a <see cref="ServiceDescriptor"/>.
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
            typeof(IRecurringJob),
            concreteClass,
            ServiceLifetime.Transient));

        services.Add(new ServiceDescriptor(
            concreteClass,
            concreteClass,
            ServiceLifetime.Transient));
    }

    /// <summary>
    /// Adds the recurring job in a hosted background service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="class">The concrete <see cref="IRecurringJob"/> to host.</param>
    private static void AddHostedService(
        IServiceCollection services,
        Type @class)
    {
        services.AddSingleton<IHostedService>(provider =>
            new RecurringJobBackgroundService((IRecurringJob)provider.GetRequiredService(@class),
                provider.GetRequiredService<IServiceScopeFactory>(),
                provider.GetRequiredService<ILogger<RecurringJobBackgroundService>>()));
    }
}
