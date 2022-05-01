using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Sprite.AspNetCore.Mvc.Abstractions;
using Sprite.Data.Exceptions;
using Sprite.DependencyInjection.Attributes;
using Sprite.Exceptions;
using Sprite.Remote;

namespace Sprite.AspNetCore.Mvc.ExceptionHandles
{
    [Component(ServiceLifetime.Transient)]
    public class SpriteExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (!ShouldHandleException(context))
            {
                return;
            }

            await HandleAndWrapException(context);
        }

        private async Task HandleAndWrapException(ExceptionContext context)
        {
            await Task.Run(() =>
            {
                context.HttpContext.Response.StatusCode = (int)GetStatusCode(context.HttpContext, context.Exception);
                context.Result = new ObjectResult(new ErrorJson
                {
                    Error = new ServiceErrorInfo(message: context.Exception.Message,
                        code: context.HttpContext.Response.StatusCode.ToString())
                });
            });
        }

        private HttpStatusCode GetStatusCode(HttpContext contextHttpContext, Exception exception)
        {
            if (exception is EntityNotFoundException)
            {
                return HttpStatusCode.NotFound;
            }

            if (exception is NotImplementedException)
            {
                return HttpStatusCode.NotImplemented;
            }

            return HttpStatusCode.InternalServerError;
        }

        protected virtual bool ShouldHandleException(ExceptionContext context)
        {
            //TODO: Create DontWrap attribute to control wrapping..?

            if (context.ActionDescriptor.IsObjectResult())
            {
                return true;
            }

            if (context.HttpContext.Request.CanAccept("application/json"))
            {
                return true;
            }

            if (context.HttpContext.Request.IsAjax())
            {
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class ErrorJson
    {
        public ServiceErrorInfo Error { get; set; }
    }
}