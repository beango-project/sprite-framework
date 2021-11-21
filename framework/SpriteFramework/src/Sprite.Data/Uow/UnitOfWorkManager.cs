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
        // public IUnitOfWork Current => _scopedUnitOfWorks?.LastOrDefault();


        public IUnitOfWork? CurrentUow => AmbientUnitOfWork.Current.UnitOfWork;


        // private readonly List<IUnitOfWork> _scopedUnitOfWorks;
        //
        // private readonly List<IUnitOfWork> _actualUnitOfWorks;

        public UnitOfWorkManager()
        {
            // _scopedUnitOfWorks = new List<IUnitOfWork>();
            // _actualUnitOfWorks = new List<IUnitOfWork>();
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
                    AmbientUnitOfWork.Current.AddUnitOfWork(newUow);
                    newUow.OnDisposed += (_, _) => { AmbientUnitOfWork.Current.AddUnitOfWork(outer); };
                    return newUow;
                }
            }
            else
            {
                var outer = CurrentUow;
                var newUow = new VirtualUnitOfWork(options, outer);
                // newUow.Outer = outer;
                newUow.OnDisposed += (_, _) => { AmbientUnitOfWork.Current.AddUnitOfWork(outer); };
                AmbientUnitOfWork.Current.AddUnitOfWork(newUow);
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
            uow.OnDisposed += (_, _) => { AmbientUnitOfWork.Current.AddUnitOfWork(outer); };
            AmbientUnitOfWork.Current.AddUnitOfWork(uow);
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