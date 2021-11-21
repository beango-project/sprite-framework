using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.AspNetCore.Mvc.Abstractions;
using Sprite.DependencyInjection;
using Sprite.DependencyInjection.Attributes;
using Sprite.Web.Attributes;

namespace Sprite.AspNetCore.Mvc.Results
{
    [Register(ServiceLifetime.Transient)]
    public class SpriteResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor))
            {
                await next();
                return;
            }

            if (context.Result is ViewResult)
            {
                await next();
                return;
            }

            var options = GetAndSetOptions(context.HttpContext.RequestServices);

            if (options.NormalizerResult && !context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo.IsDefined(typeof(NonNormalizeAttribute), true))
            {
                options.ResultHandler.Handle(context);
            }


            await next();
        }
        // public void OnResultExecuting(ResultExecutingContext context)
        // {
        //     if (!(context.ActionDescriptor is ControllerActionDescriptor))
        //     {
        //         return;
        //     }
        //
        //     if (context.Result is ViewResult)
        //     {
        //         return;
        //     }
        //
        //     var options = GetAndSetOptions(context.HttpContext.RequestServices);
        //     options.ResultHandler.Handle(context);
        // }
        //
        // public void OnResultExecuted(ResultExecutedContext context)
        // {
        // }

        // private IActionResultNormalizer HandleAndGetResultType(ResultExecutingContext context)
        // {
        //     Check.NotNull(context, nameof(context));
        //
        //     if (context.Result is ObjectResult)
        //     {
        //         return new ObjectResultNormalizer(context.HttpContext.RequestServices);
        //     }
        //
        //     if (context.Result is JsonResult)
        //     {
        //         return new JsonActionResultNormalizer();
        //     }
        //
        //     if (context.Result is EmptyResult)
        //     {
        //         return new EmptyActionResultNormalizer();
        //     }
        //
        //     return new NullActionResultNormalizer();
        // }

        private AspNetCoreMvcOptions GetAndSetOptions(IServiceProvider serviceProvider)
        {
            var swapSpace = serviceProvider.GetRequiredService<SwapSpace>();
            var mvcOptions = swapSpace.TryGet<IOptions<AspNetCoreMvcOptions>>();

            if (mvcOptions == null)
            {
                var options = serviceProvider.GetRequiredService<IOptions<AspNetCoreMvcOptions>>();
                swapSpace.Add<IOptions<AspNetCoreMvcOptions>>(options);
                return options?.Value;
            }

            return mvcOptions?.Value;
        }
    }
}