using Quartz;
using Quartz.Spi;

namespace Server.Services.Jobs;

public class JobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobFactory(IServiceCollection serviceCollection)
    {
        _serviceProvider = serviceCollection.BuildServiceProvider();
        using var sc = _serviceProvider.CreateScope();
        _ = sc.ServiceProvider.GetService<TaskManager>();
    }


    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var jobType = bundle.JobDetail.JobType;
        if (!typeof(BaseJob).IsAssignableFrom(jobType))
        {
            throw new ApplicationException("JobFactory поддерживает только джобы унаследованные от BaseJob!");
        }

        IServiceScope scope = null;
        try
        {
            scope = _serviceProvider.CreateScope();

            var jobList = scope.ServiceProvider.GetServices(typeof(BaseJob)).OfType<BaseJob>().ToList();

            var job = jobList.FirstOrDefault(j => j.GetType() == jobType);
            if (job == null)
            {
                return job;
            }

            job.ServiceScope = scope;

            return job;
        }
        catch
        {
            scope?.Dispose();

            throw;
        }
    }

    public void ReturnJob(IJob job)
    {
        var baseJob = (BaseJob)job;
        baseJob.ServiceScope?.Dispose();
    }
}