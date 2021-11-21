using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        public bool ApplyLocal { get; set; }


        protected internal IServiceCollection Services { get; }

        public void ApplySnowflake(Action<SnowflakeGeneratorOptions> options)
        {
            var snowflakeGeneratorOptions = new SnowflakeGeneratorOptions();
            options?.Invoke(snowflakeGeneratorOptions);
            var snowflakeUidProvider = new SnowflakeUidProvider(snowflakeGeneratorOptions);
            Services.TryAddSingleton(typeof(IDistributedUniqueIdGenerator), _ => new DistributedUniqueIdGenerator(snowflakeUidProvider));
        }

        public void ApplySnowflake(SnowflakeGeneratorOptions options)
        {
            var snowflakeUidProvider = new SnowflakeUidProvider(options);
            Services.TryAddSingleton(typeof(IDistributedUniqueIdGenerator), _ => new DistributedUniqueIdGenerator(snowflakeUidProvider));
        }
    }
}