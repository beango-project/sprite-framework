using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sprite.DynamicProxy
{
    public interface IMethodInvocation
    {
        object[] Arguments { get; }

        Type[] GenericArguments { get; }

        object Target { get; }

        MethodInfo Method { get; }

        MethodInfo TargetMethod { get; }

        object ReturnValue { get; set; }

        Task ProceedAsync();
    }
}