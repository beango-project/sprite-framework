using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AspectInjector.Broker;
using ImmediateReflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;
using Sprite.DependencyInjection;

namespace Sprite.AspNetCore.Mvc.Uow
{
    public class UnitOfWorkActionFilter : Attribute, IAsyncActionFilter, ITransientInjection
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private bool _delayedDispose;

        public UnitOfWorkActionFilter(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var unitOfWork = _unitOfWorkManager.Reserve("ReservedUow"); //Reserve a unit of work for use by subsequent Aspect
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor && !controllerActionDescriptor.MethodInfo.IsDefined<TransactionalAttribute>())
            {
                unitOfWork.Active(new TransactionOptions());
            }

            var result = await next();

            if (IsSuccessful(result))
            {
                if (!unitOfWork.IsCompleted)
                {
                    await unitOfWork.CompletedAsync(context.HttpContext.RequestAborted);
                }

                if (result.Result is PageResult or ViewResult or PartialViewResult or ObjectResult) //setting support lazy call (IQueryable,IAsyncEnumerable)
                {
                    context.HttpContext.Items["DelayedDisposeUow"] = unitOfWork;
                    _delayedDispose = true;
                }
                else
                {
                    await unitOfWork.DisposeAsync();
                }
            }
            else
            {
                await unitOfWork.RollbackAsync();

                if (!_delayedDispose)
                {
                    await unitOfWork.DisposeAsync();
                }
            }
        }


        protected bool IsSuccessful(ActionExecutedContext context)
        {
            return context.Exception == null || context.ExceptionHandled;
        }
    }
}