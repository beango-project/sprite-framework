using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Sprite.DynamicProxy;

namespace Sprite.Castle.DynamicProxy
{
    public class CastleAsyncInterceptorAdapter<TInterceptor> : AsyncInterceptorBase
        where TInterceptor : IAspectInterceptor
    {
        private readonly TInterceptor _interceptor;

        public CastleAsyncInterceptorAdapter(TInterceptor interceptor)
        {
            _interceptor = interceptor;
        }


        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            await _interceptor.InterceptAsync(new CastleAsyncInvocation(invocation, proceedInfo, proceed));
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var castleAsyncInvocation = new CastleAsyncInvocation<TResult>(invocation, proceedInfo, proceed);
            await _interceptor.InterceptAsync(castleAsyncInvocation);

            return (TResult) castleAsyncInvocation.ReturnValue;
        }
    }
}