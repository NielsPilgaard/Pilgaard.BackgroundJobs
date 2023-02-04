namespace Pilgaard.BackgroundJobs;

/// <summary>
/// Options for the default implementation of <see cref="BackgroundJobScheduler"/>.
/// </summary>
public sealed class BackgroundJobServiceOptions
{
    /// <summary>
    /// Gets the background job registrations.
    /// </summary>
    public ICollection<BackgroundJobRegistration> Registrations { get; } = new List<BackgroundJobRegistration>();
}
