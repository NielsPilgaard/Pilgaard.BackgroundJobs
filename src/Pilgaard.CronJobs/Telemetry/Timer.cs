using System.Diagnostics.Metrics;

namespace Pilgaard.CronJobs.Telemetry;

// Copied from https://github.com/prometheus-net/prometheus-net/blob/7eec46bbe81f5eb425e1ab9e544a9137672996df/Prometheus/TimerExtensions.cs
internal sealed class Timer : ITimer
{
    private readonly ValueStopwatch _stopwatch = ValueStopwatch.StartNew();
    private readonly Action<double> _observeDurationAction;
    internal Timer(Histogram<double> histogram)
    {
        _observeDurationAction = histogram.Record;
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
