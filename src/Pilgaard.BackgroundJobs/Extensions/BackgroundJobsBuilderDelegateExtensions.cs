using Cronos;

namespace Pilgaard.BackgroundJobs.Extensions;

public static class BackgroundJobsBuilderDelegateExtensions
{
    public static IBackgroundJobsBuilder AddJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Action job,
        CronExpression cronExpression,
        IEnumerable<string>? tags = null,
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

        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, tags));
    }

    public static IBackgroundJobsBuilder AddJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Action job,
        TimeSpan interval,
        IEnumerable<string>? tags = null,
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

        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, tags));
    }

    public static IBackgroundJobsBuilder AddJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Action job,
        DateTime scheduledTimeUtc,
        IEnumerable<string>? tags = null,
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

        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, tags));
    }

    public static IBackgroundJobsBuilder AddAsyncJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Func<CancellationToken, Task> job,
        CronExpression cronExpression,
        IEnumerable<string>? tags = null,
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
        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, tags));
    }

    public static IBackgroundJobsBuilder AddAsyncJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Func<CancellationToken, Task> job,
        TimeSpan interval,
        IEnumerable<string>? tags = null,
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
        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, tags));
    }

    public static IBackgroundJobsBuilder AddAsyncJob(
        this IBackgroundJobsBuilder builder,
        string name,
        Func<CancellationToken, Task> job,
        DateTime scheduledTimeUtc,
        IEnumerable<string>? tags = null,
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
        return builder.Add(new BackgroundJobRegistration(instance, name, timeout, tags));
    }
}
