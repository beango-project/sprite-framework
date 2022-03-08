using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sprite.UidGenerator.Providers;
using Sprite.UidGenerator.Providers.Snowflake;

namespace Sprite.UidGenerator
{
    public class DistributedUniqueIdGeneratorOptions
    {
        public DistributedUniqueIdGeneratorOptions(IServiceCollection services)
        {
            Services = services;
        }


        protected internal IServiceCollection Services { get; }


        public Type UidProviderType { get; }

        public void UseSnowflake(Action<SnowflakeGeneratorOptions> options)
        {
            var snowflakeGeneratorOptions = new SnowflakeGeneratorOptions();
            options?.Invoke(snowflakeGeneratorOptions);
            var snowflakeUidProvider = new SnowflakeUidProvider(snowflakeGeneratorOptions);
            Services.TryAddSingleton(typeof(IDistributedUniqueIdGenerator), _ => new DistributedUniqueIdGenerator(snowflakeUidProvider));
            Services.TryAddSingleton<IDistributedUniqueIdGenerator<long>>(provider => provider.GetRequiredService<IDistributedUniqueIdGenerator>());
            Services.TryAddSingleton(typeof(DistributedUniqueIdGenerator), provider => provider.GetRequiredService<IDistributedUniqueIdGenerator>());
        }

        public void UseSnowflake(SnowflakeGeneratorOptions options)
        {
            var snowflakeUidProvider = new SnowflakeUidProvider(options);
            Services.TryAddSingleton(typeof(IDistributedUniqueIdGenerator), _ => new DistributedUniqueIdGenerator(snowflakeUidProvider));
            Services.TryAddSingleton<IDistributedUniqueIdGenerator<long>>(provider => provider.GetRequiredService<IDistributedUniqueIdGenerator>());
            Services.TryAddSingleton(typeof(DistributedUniqueIdGenerator), provider => provider.GetRequiredService<IDistributedUniqueIdGenerator>());
        }
    }
}