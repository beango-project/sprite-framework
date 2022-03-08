using Microsoft.Extensions.DependencyInjection;

namespace Sprite.DependencyInjection
{
    public abstract class ServiceProviderAdapter<TContainer>
        where TContainer : class
    {
        public abstract TContainer Container { get; }

        public abstract TContainer Initialization();

        public abstract IServiceProviderFactory<TContainer> CreateServiceProviderFactory();
    }
}