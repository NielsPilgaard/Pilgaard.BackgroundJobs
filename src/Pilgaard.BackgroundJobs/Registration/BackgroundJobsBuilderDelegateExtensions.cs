using Cronos;

namespace Pilgaard.BackgroundJobs;

/// <summary>
/// Provides extension methods for adding background jobs to the <see cref="IBackgroundJobsBuilder"/>.
/// </summary>
public static class BackgroundJobsBuilderDelegateExtensions
{
    /// <summary>
    /// Adds a delegate-based background job to the builder with a specified cron expression.
    /// </summary>
    /// <param name="builder">The builder to add the job to.</param>
    /// <param name="name">The name of the job.</param>
    /// <param name="job">The delegate to be executed as the job.</param>
    /// <param name="cronExpression">The interval between recurring executions of the job.</param>
    /// <param name="timeout">The timeout for the job execution, or null to use the default timeout.</param>
    /// <returns>The updated builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder, name, or job is null.</exception>
    public static IBackgroundJobsBuilder AddJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Action job,
        CronExpression cronExpression,
        TimeSpan? timeout = default)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (job is null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        var instance = new DelegateCronJob(_ =>
        {
            job();
            return Task.CompletedTask;
        }, cronExpression);

        return builder.Add(new BackgroundJobRegistration(instance, name, timeout));
    }

    /// <summary>
    /// Adds a delegate-based background job to the builder with a specified recurring interval.
    /// </summary>
    /// <param name="builder">The builder to add the job to.</param>
    /// <param name="name">The name of the job.</param>
    /// <param name="job">The delegate to be executed as the job.</param>
    /// <param name="interval">The interval between recurring executions of the job.</param>
    /// <param name="timeout">The timeout for the job execution, or null to use the default timeout.</param>
    /// <returns>The updated builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder, name, or job is null.</exception>
    public static IBackgroundJobsBuilder AddJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Action job,
        TimeSpan interval,
        TimeSpan? timeout = default)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (job is null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        var instance = new DelegateRecurringJob(_ =>
        {
            job();
            return Task.CompletedTask;
        }, interval);

        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, isRecurringJob: true));
    }

    /// <summary>
    /// Adds a delegate-based background job to the builder with a specified scheduled time.
    /// </summary>
    /// <param name="builder">The builder to add the job to.</param>
    /// <param name="name">The name of the job.</param>
    /// <param name="job">The delegate to be executed as the job.</param>
    /// <param name="scheduledTimeUtc">The scheduled time for the job execution in UTC.</param>
    /// <param name="timeout">The timeout for the job execution, or null to use the default timeout.</param>
    /// <returns>The updated builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder, name, or job is null.</exception>
    public static IBackgroundJobsBuilder AddJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Action job,
        DateTime scheduledTimeUtc,
        TimeSpan? timeout = default)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (job is null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        var instance = new DelegateOneTimeJob(_ =>
        {
            job();
            return Task.CompletedTask;
        }, scheduledTimeUtc);

        return builder.Add(new BackgroundJobRegistration(instance, name, timeout));
    }

    /// <summary>
    /// Adds an asynchronous delegate-based background job to the builder with a specified cron expression.
    /// </summary>
    /// <param name="builder">The builder to add the job to.</param>
    /// <param name="name">The name of the job.</param>
    /// <param name="job">The delegate to be executed as the job.</param>
    /// <param name="cronExpression">The interval between recurring executions of the job.</param>
    /// <param name="timeout">The timeout for the job execution, or null to use the default timeout.</param>
    /// <returns>The updated builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder, name, or job is null.</exception>
    public static IBackgroundJobsBuilder AddAsyncJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Func<CancellationToken, Task> job,
        CronExpression cronExpression,
        TimeSpan? timeout = default)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (job is null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        var instance = new DelegateCronJob(job, cronExpression);
        return builder.Add(new BackgroundJobRegistration(instance, name, timeout));
    }

    /// <summary>
    /// Adds an asynchronous delegate-based background job to the builder with a specified recurring interval.
    /// </summary>
    /// <param name="builder">The builder to add the job to.</param>
    /// <param name="name">The name of the job.</param>
    /// <param name="job">The delegate to be executed as the job.</param>
    /// <param name="interval">The interval between recurring executions of the job.</param>
    /// <param name="timeout">The timeout for the job execution, or null to use the default timeout.</param>
    /// <returns>The updated builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder, name, or job is null.</exception>
    public static IBackgroundJobsBuilder AddAsyncJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Func<CancellationToken, Task> job,
        TimeSpan interval,
        TimeSpan? timeout = default)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (job is null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        var instance = new DelegateRecurringJob(job, interval);
        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, isRecurringJob: true));
    }

    /// <summary>
    /// Adds an asynchronous delegate-based background job to the builder with a specified scheduled time.
    /// </summary>
    /// <param name="builder">The builder to add the job to.</param>
    /// <param name="name">The name of the job.</param>
    /// <param name="job">The delegate to be executed as the job.</param>
    /// <param name="scheduledTimeUtc">The scheduled time for the job execution in UTC.</param>
    /// <param name="timeout">The timeout for the job execution, or null to use the default timeout.</param>
    /// <returns>The updated builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder, name, or job is null.</exception>
    public static IBackgroundJobsBuilder AddAsyncJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Func<CancellationToken, Task> job,
        DateTime scheduledTimeUtc,
        TimeSpan? timeout = default)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (job is null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        var instance = new DelegateOneTimeJob(job, scheduledTimeUtc);
        return builder.Add(new BackgroundJobRegistration(instance, name, timeout));
    }
}
