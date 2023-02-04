namespace Pilgaard.BackgroundJobs;

/// <summary>
/// This interface represents a background job that can be run asynchronously.
/// <para>
/// <b>This shouldn't be used directly.</b> 
/// </para>
/// <para>
/// You'll want to use either <see cref="ICronJob"/>,
/// <see cref="IRecurringJob"/> or <see cref="IOneTimeJob"/>, depending on your scheduling needs.
/// </para>
/// </summary>
public interface IBackgroundJob
{
    /// <summary>
    /// Runs the background job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task RunJobAsync(CancellationToken cancellationToken = default);
}
