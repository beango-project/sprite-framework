using System;
using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// Represents an <see cref="IServiceProviderFactory{TContainerBuilder}" /> implementation based on <see cref="IStashboxContainer" />
    /// </summary>
    public class StashboxServiceProviderFactory : IServiceProviderFactory<IStashboxContainer>
    {
        private readonly Action<IStashboxContainer> configure;
        private readonly IStashboxContainer container;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceProviderFactory" />
        /// </summary>
        /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer" />.</param>
        public StashboxServiceProviderFactory(Action<IStashboxContainer> configure)
        {
            this.configure = configure;
        }

        /// <summary>
        /// Constructs a <see cref="StashboxServiceProviderFactory" />
        /// </summary>
        /// <param name="container">An already configured <see cref="IStashboxContainer" /> instance to use.</param>
        public StashboxServiceProviderFactory(IStashboxContainer container)
        {
            this.container = container;
        }

        /// <inheritdoc />
        public IStashboxContainer CreateBuilder(IServiceCollection services)
        {
            return container != null ? services.CreateBuilder(container) : services.CreateBuilder(configure);
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(IStashboxContainer containerBuilder)
        {
            return new StashboxRequiredServiceProvider(containerBuilder);
        }
    }
}