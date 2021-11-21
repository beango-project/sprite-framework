using System;
using Sprite.Data.Persistence;

namespace Sprite.Data.Transaction
{
    public class PlatformTransactionManager : ITransactionManager
    {
        private IPersistenceVenderProvider _persistenceVender;

        private AmbientTransaction _AmbientTransaction { get; }
        public ITransactionInfo CurrentTransaction => CurrentTransaction;


        public ITransactionInfo BeginTransaction(ITransactionDescriptor descriptor)
        {
            if (descriptor.Propagation == Propagation.RequiresNew && CurrentTransaction != null)
            {
                var persistenceVender = _persistenceVender.GetPersistenceVender();
                return CreateNewTransaction(descriptor);
            }

            return null;
        }

        private ITransactionInfo CreateNewTransaction(ITransactionDescriptor descriptor)
        {
            // var  persistenceVender=_persistenceVender.GetPersistenceVender() as Ado.AdoPersistence<>
            //  var transaction = new TransactionInfo();
            //  transaction.
            throw new NotImplementedException();
        }

        public ITransactionInfo GetCurrentTransactionInfo()
        {
            return _AmbientTransaction.CurrentTransaction.Value;
        }
    }
}