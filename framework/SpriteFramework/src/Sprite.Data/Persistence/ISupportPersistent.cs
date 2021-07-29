using System.Threading;
using System.Threading.Tasks;

namespace Sprite.Data.Persistence
{
    public interface ISupportPersistent
    {
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}