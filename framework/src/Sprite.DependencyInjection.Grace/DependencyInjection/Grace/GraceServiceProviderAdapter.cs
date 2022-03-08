using Grace.DependencyInjection;
using Grace.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.DependencyInjection.Grace
{
    public class GraceServiceProviderAdapter : ServiceProviderAdapter<IInjectionScope>
    {
        private IInjectionScope _container;
        public override IInjectionScope Container { get; }

        public override IInjectionScope Initialization()
        {
            var container = new DependencyInjectionContainer();

            container.Add(new SpriteGraceConfigurationModule());

            return _container = container;
        }

        public override GraceServiceProviderFactory CreateServiceProviderFactory()
        {
            return new GraceServiceProviderFactory(_container);
        }
    }
}