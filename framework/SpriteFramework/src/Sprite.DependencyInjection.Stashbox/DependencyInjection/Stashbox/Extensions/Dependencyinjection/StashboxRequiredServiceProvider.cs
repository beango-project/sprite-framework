using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// A service provider implementation which implements <see cref="ISupportRequiredService" /> and uses Stashbox to produce services.
    /// </summary>
    public class StashboxRequiredServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable, IAsyncDisposable
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxRequiredServiceProvider" />.
        /// </summary>
        /// <param name="dependencyResolver">The stashbox dependency resolver.</param>
        public StashboxRequiredServiceProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return dependencyResolver.DisposeAsync();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            dependencyResolver.Dispose();
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return dependencyResolver.GetService(serviceType);
        }

        /// <inheritdoc />
        public object GetRequiredService(Type serviceType)
        {
            return dependencyResolver.Resolve(serviceType);
        }
    }
}