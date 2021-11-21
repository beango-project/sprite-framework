using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Sprite.Data.Persistence;

namespace Sprite.Data.EntityFrameworkCore.Persistence
{
    public interface IEfCorePersistence<TDbContext> : IVendor, ISupportPersistent, ISupportTransaction
        where TDbContext : DbContext
    {
        public TDbContext DbContext { get; }

        public IDbContextTransaction DbContextTransaction { get; }
    }
}