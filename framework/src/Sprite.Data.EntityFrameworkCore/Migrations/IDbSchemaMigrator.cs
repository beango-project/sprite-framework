using System.Threading.Tasks;

namespace Sprite.Data.EntityFrameworkCore.Migrations
{
    /// <summary>
    /// 数据库架构迁移器
    /// </summary>
    public interface IDbSchemaMigrator
    {
        /// <summary>
        /// 迁移
        /// </summary>
        /// <returns></returns>
        Task MigrateAsync();
    }
}