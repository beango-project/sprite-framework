using System.Threading;
using System.Threading.Tasks;

namespace Sprite.Caching
{
    public interface IBackgroundWork
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}