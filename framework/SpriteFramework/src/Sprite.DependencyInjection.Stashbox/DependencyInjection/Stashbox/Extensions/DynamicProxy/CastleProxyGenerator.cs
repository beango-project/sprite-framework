using Castle.DynamicProxy;

namespace Sprite.DependencyInjection.Stashbox.Extensions.DynamicProxy
{
    public class CastleProxyGenerator
    {
        private static volatile ProxyGenerator _proxyGeneratorInstance;

        private CastleProxyGenerator()
        {
        }

        public static ProxyGenerator Build()
        {
            if (_proxyGeneratorInstance == null)
            {
                lock (_proxyGeneratorInstance)
                {
                    _proxyGeneratorInstance = new ProxyGenerator();
                }
            }

            return _proxyGeneratorInstance;
        }
    }
}