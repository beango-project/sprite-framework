using System;
using System.Reflection;
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
    // [Component(ServiceLifetime.Transient)]
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

            if (options.NormalizerResult && !context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo.IsDefinedAttribute(typeof(NonNormalizeAttribute), true))
            {
                await options.ResultHandler.HandleAsync(context);
            }


            await next();
        }

        private AspNetCoreMvcOptions GetAndSetOptions(IServiceProvider serviceProvider)
        {
            var swapSpace = serviceProvider.GetRequiredService<SwapSpace>();
            swapSpace.TryGet<IOptions<AspNetCoreMvcOptions>>(out var mvcOptions);

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