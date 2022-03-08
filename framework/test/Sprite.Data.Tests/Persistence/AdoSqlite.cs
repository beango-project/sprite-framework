using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Sprite.Data.Ado;

namespace Sprite.Data.Tests.Persistence
{
    public class AdoSqlite : Ado<SqliteConnection>
    {
        public AdoSqlite([NotNull] SqliteConnection dbConnection) : base(dbConnection)
        {
        }
    }
}