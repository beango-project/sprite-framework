using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Sprite.DynamicProxy;

namespace Sprite.Castle.DynamicProxy
{
    public abstract class CastleAsyncInvocationBase : IMethodInvocation, IInvocation
    {
        /// <summary>
        /// 这个是Castle的拦截器
        /// </summary>
        protected readonly IInvocation _invocationImplementation;

        protected CastleAsyncInvocationBase(IInvocation invocation)
        {
            _invocationImplementation = invocation;
        }

        public object GetArgumentValue(int index)
        {
            return _invocationImplementation.GetArgumentValue(index);
        }

        public MethodInfo GetConcreteMethod()
        {
            return _invocationImplementation.GetConcreteMethod();
        }

        public MethodInfo GetConcreteMethodInvocationTarget()
        {
            return _invocationImplementation.GetConcreteMethodInvocationTarget();
        }

        public void Proceed()
        {
            _invocationImplementation.Proceed();
        }

        public IInvocationProceedInfo CaptureProceedInfo()
        {
            return _invocationImplementation.CaptureProceedInfo();
        }

        public void SetArgumentValue(int index, object value)
        {
            _invocationImplementation.SetArgumentValue(index, value);
        }

        public object InvocationTarget => _invocationImplementation.InvocationTarget;
        public MethodInfo MethodInvocationTarget => _invocationImplementation.MethodInvocationTarget;
        public object Proxy => _invocationImplementation.Proxy;
        public Type TargetType => _invocationImplementation.TargetType;

        public object[] Arguments => _invocationImplementation.Arguments;
        public Type[] GenericArguments => _invocationImplementation.GenericArguments;
        public object Target => _invocationImplementation.InvocationTarget ?? _invocationImplementation.MethodInvocationTarget;
        public MethodInfo Method => _invocationImplementation.Method;
        public MethodInfo TargetMethod => _invocationImplementation.MethodInvocationTarget ?? Method;
        public object ReturnValue { get; set; }

        public abstract Task ProceedAsync();
    }
}