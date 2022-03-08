using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Sprite.Data.EntityFrameworkCore.Audit
{
    public abstract class AuditDbContextBase<TDbContext> : DbContextBase<TDbContext>
        where TDbContext : DbContext
    {
        protected AuditDbContextBase([NotNull] DbContextOptions<TDbContext> options) : base(options)
        {
        }
        
        public DbSet<AuditLog> AuditLogs { get; }
        
        public DbSet<EntityChangeHistory> EntityChangeHistories { get; }
    }
}