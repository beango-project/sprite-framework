using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Data.Persistence;
using Sprite.Data.Transaction;
using Sprite.DependencyInjection;

namespace Sprite.Data.Uow
{
    public class UnitOfWorkManager : ISingletonInjection
    {
        private readonly IAmbientUnitOfWork _ambientUnitOfWork;

        private readonly IServiceScopeFactory _serviceScopeFactory;


        private AsyncLocal<IVendor> _vendorCell = new AsyncLocal<IVendor>();

        public UnitOfWorkManager(IServiceScopeFactory serviceScopeFactory, IAmbientUnitOfWork ambientUnitOfWork)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _ambientUnitOfWork = ambientUnitOfWork;
        }

        public IUnitOfWork CurrentUow => GetCurrentUnitOfWork();

        public IVendor CurrentVendor => GetCurrentVendor();

        public IUnitOfWork Begin(TransactionOptions options)
        {
            switch (options.Propagation)
            {
                case TransactionPropagation.Required:
                    if (CurrentUow != null)
                    {
                        var virtualUnitOfWork = CreateVirtualUnitOfWork();
                        _ambientUnitOfWork.SetUnitOfWork(virtualUnitOfWork);
                        return virtualUnitOfWork;
                    }

                    var newUnitOfWork = new UnitOfWork(CurrentVendor, null);
                    _ambientUnitOfWork.SetUnitOfWork(newUnitOfWork);
                    return newUnitOfWork;

                case TransactionPropagation.RequiresNew:
                    if (CurrentUow != null)
                    {
                        //挂起当前工作单元
                    }

                    var serviceScope = _serviceScopeFactory.CreateScope();
                    try
                    {
                        var vendorType = CurrentVendor.GetType();
                        var implVendorType = vendorType.GenericTypeArguments[0];
                        var adapterType = typeof(PersistenceVendorAdapter<>).MakeGenericType(implVendorType);
                        var isAssignable = vendorType.IsAssignableTo(adapterType);
                        //这里用反射获取真正的DbContext
                        var vendor = serviceScope.ServiceProvider.GetRequiredService(implVendorType);

                        var instance = Activator.CreateInstance(vendorType, vendor);

                        var unitOfWork = new UnitOfWork((IVendor) instance, null);

                        //工作单元销毁时 持久化提供商也销毁
                        unitOfWork.Disposed += (_, _) => { serviceScope.Dispose(); };

                        _ambientUnitOfWork.SetUnitOfWork(unitOfWork);
                        return unitOfWork;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }


                    break;
            }

            return null;
        }

        public IUnitOfWork CreateVirtualUnitOfWork()
        {
            return new VirtualUnitOfWork(CurrentUow);
        }

        private IVendor GetCurrentVendor()
        {
            // while (!_vendorCell.Value.IsDispose)
            // {
            //     return _vendorCell.Value;
            // }
            return _vendorCell.Value;
            // return null;
        }

        public void SetVendor(IVendor vendor)
        {
            _vendorCell.Value = vendor;
        }

        private IUnitOfWork GetCurrentUnitOfWork()
        {
            var uow = _ambientUnitOfWork.UnitOfWork;

            //Skip reserved unit of work
            // while (uow != null && (uow.IsDisposed || uow.IsCompleted))
            // {
            //     return uow;
            // }
            //
            // return null;
            return uow;
        }
    }
}