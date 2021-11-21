using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Sprite.DynamicProxy;

namespace Sprite.Castle.DynamicProxy
{
    public class CastleAsyncInvocation : CastleAsyncInvocationBase, IMethodInvocation
    {
        public CastleAsyncInvocation(IInvocation invocation, IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed)
            : base(invocation)
        {
            ProceedInfo = proceedInfo;
            Proceed = proceed;
        }

        protected IInvocationProceedInfo ProceedInfo { get; }
        protected Func<IInvocation, IInvocationProceedInfo, Task> Proceed { get; }

        public override async Task ProceedAsync()
        {
            await Proceed(_invocationImplementation, ProceedInfo);
        }
    }
}