namespace Pilgaard.BackgroundJobs;

internal static class TypeExtensions
{
    internal static bool ImplementsRecurringJob(this Type jobType) => jobType.GetInterfaces().Any(@interface => @interface == typeof(IRecurringJob));
}
