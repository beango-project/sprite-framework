using System.Threading.Tasks;

namespace Sprite.Scheduling.Abstractions
{
    public interface IAsyncSchedulingJob<T>
    {
        Task ExecuteAsync(T t);
    }
}