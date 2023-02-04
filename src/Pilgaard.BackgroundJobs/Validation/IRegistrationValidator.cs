namespace Pilgaard.BackgroundJobs;

/// <summary>
/// A validator for background job registrations.
/// </summary>
internal interface IRegistrationValidator
{
    /// <summary>
    /// Verifies that all <paramref name="registrations"/> are valid.
    /// </summary>
    /// <param name="registrations"></param>
    void Validate(ICollection<BackgroundJobRegistration> registrations);
}
