using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AspectInjector.Broker;
using Microsoft.AspNetCore.Mvc.Filters;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;
using Sprite.DependencyInjection;

namespace Sprite.AspNetCore.Mvc.Uow
{
    public class UnitOfWorkActionFilter : Attribute, IAsyncActionFilter, IScopeInjection
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkActionFilter(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IUnitOfWork unitOfWork = null;
            if (!context.ActionDescriptor.GetType().IsDefined(typeof(TransactionalAttribute)))
            {
                unitOfWork = _unitOfWorkManager.Begin(new TransactionOptions() { Propagation = Propagation.Required });
            }

            await next();
            await unitOfWork?.CompletedAsync();
            unitOfWork?.Dispose();
        }
    }
}