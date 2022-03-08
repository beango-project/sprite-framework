using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sprite.Data.Transaction;
using Sprite.DependencyInjection;

namespace Sprite.Data.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager, ISingletonInjection
    {
        public IUnitOfWork? CurrentUow => GetUnitOfWork();


        public UnitOfWorkManager()
        {
        }

        public IUnitOfWork Begin(TransactionOptions options = null)
        {
            if (options == null)
            {
                options = new TransactionOptions();
            }

            switch (options.Propagation)
            {
                case Propagation.Required:
                    return FindOrBeginVirtualUnitOfWork(options, true) ?? BeginNewUnitOfWork(options);
                case Propagation.Auto:
                case Propagation.Supports:
                    if (CurrentUow != null)
                    {
                        return FindOrBeginVirtualUnitOfWork(options, CurrentUow.IsSupportTransaction) ?? BeginNewUnitOfWork(options);
                    }
                    else
                    {
                        return BeginNewUnitOfWork(options);
                    }
                case Propagation.RequiresNew:
                    return BeginNewUnitOfWork(options);
                case Propagation.Mandatory:
                    return FindOrBeginVirtualUnitOfWork(options, true) ?? throw new InvalidOperationException("must  tx");
                case Propagation.Nested:
                    return FindOrBeginVirtualUnitOfWork(options, true) ?? throw new InvalidOperationException("There is currently no working unit of work and cannot be nested");
                case Propagation.NotSupported:
                    return FindOrBeginVirtualUnitOfWork(options, false) ?? BeginNewUnitOfWork(options);
                case Propagation.Never:
                    return FindOrCreateNonTransactionalUnitOfWork(options);
                default: throw new NotImplementedException();
            }
        }

        public IUnitOfWork Reserve(string reservationName)
        {
            Check.NotNullOrEmpty(reservationName, nameof(reservationName));

            if (CurrentUow != null && CurrentUow.ReservationKey == reservationName)
            {
                //TODO Setting TransactionOptions
                return new VirtualUnitOfWork(null, CurrentUow);
            }

            var outer = CurrentUow;
            var uow = new UnitOfWork(reservationName);
            uow.Outer = outer;
            uow.OnDisposed += (_, _) => { AmbientUnitOfWork.Current.SetUnitOfWork(outer); };
            AmbientUnitOfWork.Current.SetUnitOfWork(uow);

            return uow;
        }

        public bool TryBeginReserved(string reservationKey, TransactionOptions options)
        {
            var uow = AmbientUnitOfWork.Current.UnitOfWork;
            while (uow != null && !(uow.IsReserved && uow.ReservationKey == reservationKey))
            {
                uow = uow.Outer;
            }

            if (uow == null)
            {
                return false;
            }

            uow.Active(options);

            return true;
        }

        private IUnitOfWork GetUnitOfWork()
        {
            var uow = AmbientUnitOfWork.Current.UnitOfWork;

            //Skip reserved unit of work
            while (uow != null && (uow.IsReserved || uow.IsDisposed || uow.IsCompleted))
            {
                uow = uow.Outer;
            }

            return uow;
        }

        private IUnitOfWork FindOrBeginVirtualUnitOfWork(TransactionOptions options, bool isTransactional)
        {
            if (CurrentUow == null)
            {
                return null;
            }

            if (CurrentUow.Options.Propagation == Propagation.Never)
            {
                if (options.Propagation is Propagation.Required or Propagation.RequiresNew or Propagation.Mandatory or
                    Propagation.Nested)
                {
                    throw new Exception("由于事务传播配置为Never,无法开启事务");
                }
            }

            if (CurrentUow.IsSupportTransaction != isTransactional)
            {
                return null;
            }

            if (CurrentUow is VirtualUnitOfWork virtualUow)
            {
                if (virtualUow is { IsDisposed: false, IsCompleted: false })
                {
                    var outer = CurrentUow;
                    var newUow = new VirtualUnitOfWork(options, virtualUow.BaseUow);
                    // newUow.Outer = outer;
                    AmbientUnitOfWork.Current.SetUnitOfWork(newUow);
                    newUow.OnDisposed += (_, _) => { AmbientUnitOfWork.Current.SetUnitOfWork(outer); };
                    return newUow;
                }
            }
            else
            {
                var outer = CurrentUow;
                var newUow = new VirtualUnitOfWork(options, outer);
                // newUow.Outer = outer;
                newUow.OnDisposed += (_, _) => { AmbientUnitOfWork.Current.SetUnitOfWork(outer); };
                AmbientUnitOfWork.Current.SetUnitOfWork(newUow);
                return newUow;
            }
            //
            // if (Current is VirtualUnitOfWork virtualUow)
            // {
            //     var uow = _actualUnitOfWorks.LastOrDefault(u => u.Id == virtualUow.BaseUow.Id);
            //     if (uow is { IsDisposed: false, IsCompleted: false })
            //     {
            //         var newUow = new VirtualUnitOfWork(uow);
            //         _scopedUnitOfWorks.Add(newUow);
            //         newUow.OnDisposed += (_, _) => { _scopedUnitOfWorks.Remove(newUow); };
            //         return newUow;
            //     }
            // }
            // else
            // {
            //     var newUow = new VirtualUnitOfWork(Current);
            //     _scopedUnitOfWorks.Add(newUow);
            //     newUow.OnDisposed += (_, _) => { _scopedUnitOfWorks.Remove(newUow); };
            //     return newUow;
            // }

            return null;
        }


        private IUnitOfWork BeginNewUnitOfWork(TransactionOptions options)
        {
            var outer = CurrentUow;
            var uow = new UnitOfWork(options);
            uow.Outer = outer;
            uow.OnDisposed += (_, _) => { AmbientUnitOfWork.Current.SetUnitOfWork(outer); };
            AmbientUnitOfWork.Current.SetUnitOfWork(uow);
            // uow.OnDisposed += (_, _) =>
            // {
            //     _actualUnitOfWorks.Remove(uow);
            //     _scopedUnitOfWorks.Remove(uow);
            // };
            // _scopedUnitOfWorks.Add(uow);
            // _actualUnitOfWorks.Add(uow);

            return uow;
        }

        private IUnitOfWork FindOrCreateNonTransactionalUnitOfWork(TransactionOptions options)
        {
            var outer = CurrentUow;
            // if (outer != null && outer.IsSupportTransaction)
            // {
            //     throw new Exception("Propagation_Never: 以非事务方式执行操作，如果当前事务存在则抛出异常");
            // }

            return BeginNewUnitOfWork(options);
        }
    }
}