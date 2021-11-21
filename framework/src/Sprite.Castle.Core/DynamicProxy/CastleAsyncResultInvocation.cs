using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Sprite.Castle.DynamicProxy
{
    public class CastleAsyncInvocation<TResult> : CastleAsyncInvocationBase
    {
        public CastleAsyncInvocation(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed) :
            base(invocation)
        {
            ProceedInfo = proceedInfo;
            Proceed = proceed;
        }

        protected IInvocationProceedInfo ProceedInfo { get; }
        protected Func<IInvocation, IInvocationProceedInfo, Task<TResult>> Proceed { get; }

        public override async Task ProceedAsync()
        {
            ReturnValue = await Proceed(_invocationImplementation, ProceedInfo);
        }
    }
}