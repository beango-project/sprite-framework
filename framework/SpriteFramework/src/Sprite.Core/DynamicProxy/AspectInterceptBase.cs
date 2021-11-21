using System.Threading.Tasks;

namespace Sprite.DynamicProxy
{
    public abstract class AspectInterceptBase : IAspectInterceptor
    {
        public abstract Task InterceptAsync(IMethodInvocation invocation);
    }
}