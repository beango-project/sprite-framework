using System.Threading.Tasks;
using Sprite.Aspects;

namespace Sprite.DynamicProxy
{
    public interface IAspectInterceptor : IAspect
    {
        Task InterceptAsync(IMethodInvocation invocation);
    }
}