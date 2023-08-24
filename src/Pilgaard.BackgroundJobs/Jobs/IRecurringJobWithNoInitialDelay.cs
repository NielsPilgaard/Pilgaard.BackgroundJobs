namespace Pilgaard.BackgroundJobs;

/// <summary>
/// This interface represents a background job that runs at a specified interval, and immediately on startup.
/// </summary>
public interface IRecurringJobWithNoInitialDelay : IRecurringJob { }
