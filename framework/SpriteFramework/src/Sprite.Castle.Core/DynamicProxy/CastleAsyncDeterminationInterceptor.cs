using Castle.DynamicProxy;
using Sprite.DynamicProxy;

namespace Sprite.Castle.DynamicProxy
{
    public class CastleAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor, IInterceptor where TInterceptor : IAspectInterceptor
    {
        public CastleAsyncDeterminationInterceptor(TInterceptor interceptor) : base(new CastleAsyncInterceptorAdapter<TInterceptor>(interceptor))
        {
            // var proxy = new ProxyGenerator();
            // proxy.CreateInterfaceProxyWithTarget()
        }
    }
}