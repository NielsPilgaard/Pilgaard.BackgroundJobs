using System.Text;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// The default implementation of <see cref="IRegistrationValidator"/>.
/// </summary>
internal sealed class RegistrationValidator : IRegistrationValidator
{
    /// <summary>
    /// Scan the <see cref="ICollection{BackgroundJobRegistration}"/> for duplicate names
    /// to provide an error if there are duplicates.
    /// </summary>
    /// <param name="registrations">The background job registrations.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Validate(ICollection<BackgroundJobRegistration> registrations)
    {
        StringBuilder? builder = null;
        var distinctRegistrations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var registration in registrations)
        {
            if (!distinctRegistrations.Add(registration.Name))
            {
                builder ??= new StringBuilder("Duplicate health checks were registered with the name(s): ");

                builder.Append(registration.Name).Append(", ");
            }
        }

        if (builder is not null)
            throw new ArgumentException(builder.ToString(0, builder.Length - 2), nameof(registrations));
    }
}
