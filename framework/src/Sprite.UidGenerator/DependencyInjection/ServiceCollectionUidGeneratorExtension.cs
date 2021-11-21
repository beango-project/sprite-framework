using System;
using Sprite.UidGenerator;
using Sprite.UidGenerator.Providers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionUidGeneratorExtension
    {
        public static void AddUidGenerator(this IServiceCollection services, Action<UniqueIdGeneratorOptions> options = null)
        {
            var uniqueIdGeneratorOptions = new UniqueIdGeneratorOptions()
            {
                UniqueIdGeneratorType = UniqueIdGeneratorType.None
            };

            options?.Invoke(uniqueIdGeneratorOptions);

            services.Configure(options);

            switch (uniqueIdGeneratorOptions.UniqueIdGeneratorType)
            {
                case UniqueIdGeneratorType.None:
                    services.AddSingleton<IUniqueIdGenerator>(new UniqueIdGenerator(new GuidProvider()));
                    break;
            }
        }

        public static void AddDistributedUidGenerator(this IServiceCollection services, Action<DistributedUniqueIdGeneratorOptions> options)
        {
            var distributedUniqueIdGeneratorOptions = new DistributedUniqueIdGeneratorOptions(services);
            options?.Invoke(distributedUniqueIdGeneratorOptions);
            services.PostConfigure<UniqueIdGeneratorOptions>(options => { options.IsDistributed = true; });
            services.Configure(options);
        }
    }
}