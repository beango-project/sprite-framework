using System.Threading.Tasks;

namespace Sprite.Scheduling.Abstractions
{
    public interface ISchedulingJobManager
    {
        Task ScheduleJobAsync<TJob, TContext>(TJob job, TContext context);

        Task ScheduleJobAsync<TJob, TContext>(TContext context);

        Task RemoveAsync();
    }
}