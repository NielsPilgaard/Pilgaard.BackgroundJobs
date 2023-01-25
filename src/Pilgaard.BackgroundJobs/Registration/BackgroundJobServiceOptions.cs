namespace Pilgaard.BackgroundJobs;

public sealed class BackgroundJobServiceOptions
{
    public ICollection<BackgroundJobRegistration> Registrations { get; } = new List<BackgroundJobRegistration>();
}
