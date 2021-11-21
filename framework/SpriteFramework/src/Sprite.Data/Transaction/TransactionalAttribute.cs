using System;
using System.Data;
using System.Linq;
using AspectInjector.Broker;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Aspects;
using Sprite.Data.Uow;
using Sprite.DependencyInjection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Data.Transaction
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    [Injection(typeof(UnitOfWorkAspect))]
    public class TransactionalAttribute : Attribute
    {
        public Propagation Propagation { get; set; } = Propagation.Required;

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// 超时
        /// </summary>
        public TimeSpan? Timeout { get; set; }


        // private static readonly IUnitOfWorkManager _unitOfWorkManager = ApplicationServices.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

        public TransactionalAttribute(Propagation propagation = Propagation.Required)
        {
            Propagation = propagation;
        }

        // protected override void OnBefore(AspectEventArgs eventArgs)
        // {
        //     TransactionOptions options = new TransactionOptions()
        //     {
        //         TransactionPropagation = Propagation,
        //         IsolationLevel = IsolationLevel,
        //         Timeout = Timeout
        //     };
        //     _unitOfWorkManager.Begin(options);
        // }
        //
        // protected override void OnAfter(AspectEventArgs eventArgs)
        // {
        //     base.OnAfter(eventArgs);
        // }
        // public override Type InjectionType => typeof(UnitOfWorkAspect);
    }
}