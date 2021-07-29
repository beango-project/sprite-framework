using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Sprite.Data.Ado.Uow
{
    public class UowAdoDbSessionProvider<TDbConnection> : IDbSessionProvider<TDbConnection>
        where TDbConnection : DbConnection, new()
    {
        public TDbConnection GetDbConnection()
        {
            new TDbConnection().Open();
            throw new NotImplementedException();
        }

        public Task<TDbConnection> GetDbConnectionAsync()
        {
            throw new NotImplementedException();
        }

        private TDbConnection CreateDbConnection()
        {
            // IUnitOfWork unitOfWork;
            // var adoPersistence = new AdoPersistence<TDbConnection>(new TDbConnection());
            // unitOfWork.SetPersistenceVender(adoPersistence);
            throw new NotImplementedException();
        }
    }
}