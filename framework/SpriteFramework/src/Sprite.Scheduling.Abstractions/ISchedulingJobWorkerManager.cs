using System.Threading.Tasks;

namespace Sprite.Scheduling.Abstractions
{
    public interface ISchedulingJobWorkerManager
    {
        void Add(ISchedulingJobWorker jobWorker);

        void Remove(ISchedulingJobWorker jobWorker);

        Task StartByAsync(ISchedulingJobWorker jobWorker);
    }
}