using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sprite.Data.EntityFrameworkCore
{
    public interface IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        [Obsolete("Use GetDbContextAsync method.")]
        TDbContext GetDbContext();

        Task<TDbContext> GetDbContextAsync();
    }
}