using System.Collections.Generic;

namespace Sprite.Scheduling.Abstractions
{
    public interface ISchedulingJobStore
    {
        void Add(SchedulingJobDetail jobDetail);

        List<SchedulingJobDetail> GetScheduledJobs();
    }
}