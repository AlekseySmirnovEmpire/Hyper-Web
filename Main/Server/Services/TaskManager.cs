using Quartz;
using Quartz.Impl.Matchers;
using Server.Services.Jobs;

namespace Server.Services;

public class TaskManager
{
    private readonly IScheduler _scheduler;
    private readonly ILogger<TaskManager> _logger;
    private static bool _jobCreated;

    public TaskManager(
        ILogger<TaskManager> logger,
        IScheduler scheduler,
        IEnumerable<BaseJob> jobList)
    {
        _logger = logger;
        _scheduler = scheduler;

        if (_jobCreated)
        {
            return;
        }
        
        jobList.ToList().ForEach(Add);

        _jobCreated = true;
    }

    private void Execute(string codeKey)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(SchedulerConstants.DefaultGroup))
            .Result;
        var jobKey = jobKeys.FirstOrDefault(key => key.Name == codeKey);
        if (jobKey is null)
        {
            _logger.LogWarning($"В шедулере не зарегистрирована джоба с ключем '{codeKey}'");
            return;
        }

        try
        {
            _scheduler.TriggerJob(jobKey).Wait();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Не запустилась джоба с ключем '{codeKey}', ошибка: {ex.Message}");
        }
    }

    private void Add(BaseJob job)
    {
        var jobKey = new JobKey(job.ToString());
        try
        {
            if (_scheduler.CheckExists(jobKey).Result)
            {
                _logger.LogWarning($"Джоба с ключем {jobKey} уже сущеcтвует!");
                return;
            }

            var jobDetail = JobBuilder.Create(job.GetType())
                .WithIdentity(jobKey)
                .Build();

            if (job.ScheduleJobTrigger is not null)
            {
                _scheduler.ScheduleJob(jobDetail, job.ScheduleJobTrigger.Build());
            }
            else
            {
                _scheduler.ScheduleJob(
                    jobDetail,
                    TriggerBuilder.Create()
                        .WithCalendarIntervalSchedule(x => x.WithIntervalInYears(1000))
                        .StartAt(DateTimeOffset.Now.AddYears(1000))
                        .Build());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Таск менеджер не смог добавить джобу с ключем {jobKey}, ошибка: {ex.Message}");
        }
    }
}