using System.Diagnostics.Metrics;
using Pilgaard.RecurringJobs.Telemetry;
using Timer = Pilgaard.RecurringJobs.Telemetry.Timer;

namespace Pilgaard.RecurringJobs.Extensions;

public static class HistogramExtensions
{
    /// <summary>
    /// Enables you to easily report elapsed seconds in the value of a <see cref="Histogram{T}"/>.
    /// <para>
    /// Dispose of the returned instance to report the elapsed duration.
    /// </para>
    /// </summary>
    public static ITimer NewTimer(this Histogram<double> histogram, params KeyValuePair<string, object?>[] tags) => new Timer(histogram, tags);
}
