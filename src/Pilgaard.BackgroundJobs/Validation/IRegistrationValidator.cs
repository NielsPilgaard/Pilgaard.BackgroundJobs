namespace Pilgaard.BackgroundJobs;

internal interface IRegistrationValidator
{
    void Validate(ICollection<BackgroundJobRegistration> registrations);
}
