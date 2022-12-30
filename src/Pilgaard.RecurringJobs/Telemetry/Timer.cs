using System.Diagnostics.Metrics;

namespace Pilgaard.RecurringJobs.Telemetry;

// Copied from https://github.com/prometheus-net/prometheus-net/blob/7eec46bbe81f5eb425e1ab9e544a9137672996df/Prometheus/TimerExtensions.cs
internal sealed class Timer : ITimer
{
    private readonly ValueStopwatch _stopwatch = ValueStopwatch.StartNew();
    private readonly Action<double> _observeDurationAction;
    internal Timer(Histogram<double> histogram, params KeyValuePair<string, object?>[] tags)
    {
        _observeDurationAction = duration => histogram.Record(duration, tags);
    }

    public void Dispose() => ObserveDuration();

    public TimeSpan ObserveDuration()
    {
        var duration = _stopwatch.GetElapsedTime();

        _observeDurationAction.Invoke(duration.TotalSeconds);

        return duration;
    }
}

public interface ITimer : IDisposable
{
    /// <summary>
    /// Observes the duration and returns the observed value.
    /// </summary>
    TimeSpan ObserveDuration();
}