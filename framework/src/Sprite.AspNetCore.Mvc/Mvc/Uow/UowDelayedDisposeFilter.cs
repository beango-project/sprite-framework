using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;
using Sprite.DependencyInjection;

namespace Sprite.AspNetCore.Mvc.Uow;

public class UowDelayedDisposeFilter : IAsyncAlwaysRunResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        await next();
        if (context.HttpContext.Items.ContainsKey("DelayedDisposeUow"))
        {
            var unitOfWork = context.HttpContext.Items["DelayedDisposeUow"] as IUnitOfWork;
            if (!unitOfWork.IsDisposed)
            {
                await unitOfWork.DisposeAsync();
            }
        }
    }
}