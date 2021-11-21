using System.Threading;
using System.Threading.Tasks;

namespace Sprite.Data.Persistence
{
    public interface ISupportSavepoint
    {
        void Save(string SavepointName);

        Task SaveAsync(string SavepointName, CancellationToken cancellationToken = default);

        void Rollback(string SavepointName);

        Task RollBackAsync(string SavepointName, CancellationToken cancellationToken = default);
    }
}