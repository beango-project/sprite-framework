using System.Data.Common;
using System.Threading.Tasks;

namespace Sprite.Data.Ado.Uow
{
    public interface IDbSessionProvider<TDbConnection>
        where TDbConnection : DbConnection, new()
    {
        TDbConnection GetDbConnection();

        Task<TDbConnection> GetDbConnectionAsync();
    }
}