using System.Threading.Tasks;

namespace Sprite.Data.EntityFrameworkCore.Migrations
{
    public interface IDbMigrator
    {
        /// <summary>
        /// 迁移
        /// </summary>
        /// <returns></returns>
        Task MigrateAsync();
    }
}