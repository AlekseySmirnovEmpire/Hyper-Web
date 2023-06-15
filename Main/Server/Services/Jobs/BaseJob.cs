using Quartz;

namespace Server.Services.Jobs;

public abstract class BaseJob : IJob
{
    private readonly string _cronTriggerString;
    private static readonly Dictionary<string, object> LockJob = new();

    public IServiceScope ServiceScope { get; set; }

    protected BaseJob(string jobName, string cronTrigger)
    {
        if (!LockJob.TryGetValue(jobName, out _))
        {
            LockJob.Add(jobName, new object());
        }

        _cronTriggerString = cronTrigger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        if (!LockJob.TryGetValue(ToString(), out var jobLock))
        {
            return Task.CompletedTask;
        }

        if (Monitor.TryEnter(jobLock))
        {
            try
            {
                Execute().Wait();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
            finally
            {
                Monitor.Exit(jobLock);
            }
        }
        
        return Task.CompletedTask;
    }

    public TriggerBuilder? ScheduleJobTrigger =>
        TriggerBuilder.Create()
            .WithCronSchedule(_cronTriggerString, builder => builder.InTimeZone(TimeZoneInfo.Local))
            .StartNow();

    protected abstract Task Execute();

    protected abstract void LogError(string exMessage);

    public new abstract string ToString();
}