using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sprite.Data.Uow;
using Sprite.DependencyInjection;

namespace Sprite.AspNetCore.Mvc.Uow
{
    public class UnitOfWorkMiddleware : IMiddleware, ITransientInjection
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkMiddleware(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                await next(context);
                await unitOfWork.CompletedAsync(context.RequestAborted);
            }
        }
    }
}