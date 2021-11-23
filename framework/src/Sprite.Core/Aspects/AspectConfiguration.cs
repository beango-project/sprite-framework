using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection.Attributes;
using Sprite.DynamicProxy;

namespace Sprite.Aspects
{
    [Component(ServiceLifetime.Singleton)]
    public class AspectConfiguration
    {
        public AspectConfiguration(IOptions<InvocationOptions> options)
        {
            Options = options.Value;
        }

        protected InvocationOptions Options { get; }
    }
}