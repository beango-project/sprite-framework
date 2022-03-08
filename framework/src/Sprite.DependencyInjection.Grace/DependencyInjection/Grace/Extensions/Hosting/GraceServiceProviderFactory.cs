using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection.Grace;

namespace Grace.Extensions.Hosting
{
    public class GraceServiceProviderFactory : IServiceProviderFactory<IInjectionScope>
    {
        private readonly IInjectionScope _containerBuilder;


        public GraceServiceProviderFactory(IInjectionScope containerBuilder = null)
        {
            _containerBuilder = containerBuilder ?? new DependencyInjectionContainer();
        }

        /// <summary>
        /// Creates a container builder from an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <returns>A container builder that can be used to create an <see cref="T:System.IServiceProvider" />.</returns>
        public IInjectionScope CreateBuilder(IServiceCollection services)
        {
            var container = _containerBuilder;

            container.Populate(services);

            return container;
        }

        /// <summary>
        /// Creates an <see cref="T:System.IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder</param>
        /// <returns>An <see cref="T:System.IServiceProvider" /></returns>
        public IServiceProvider CreateServiceProvider(IInjectionScope containerBuilder)
        {
            return _containerBuilder.Locate<IServiceProvider>();
        }
    }
}