using System.Diagnostics.Metrics;

namespace Pilgaard.RecurringJobs.Extensions;

internal static class HistogramExtensions
{
    /// <summary>
    /// Enables you to easily report elapsed seconds in the value of a <see cref="Histogram{T}"/>.
    /// <para>
    /// Dispose of the returned instance to report the elapsed duration.
    /// </para>
    /// </summary>
    internal static ITimer NewTimer(this Histogram<double> histogram, params KeyValuePair<string, object?>[] tags) => new Timer(histogram, tags);
}
