using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// Represents a service scope which uses Stashbox.
    /// </summary>
    public class StashboxServiceScope : IServiceScope, IAsyncDisposable
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceScope" />.
        /// </summary>
        /// <param name="dependencyResolver">The stashbox dependency resolver.</param>
        public StashboxServiceScope(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
            ServiceProvider = new StashboxRequiredServiceProvider(dependencyResolver);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return dependencyResolver.DisposeAsync();
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            dependencyResolver.Dispose();
        }
    }
}