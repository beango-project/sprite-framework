using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Data.DependencyInjection;
using Sprite.Data.EntityFrameworkCore;
using Sprite.Data.EntityFrameworkCore.Persistence;
using Sprite.Data.EntityFrameworkCore.Repositories;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionEfCoreExtensions
    {
        public static IServiceCollection AddUowAndRepositories<TDbContext>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TDbContext : DbContextBase<TDbContext>
        {
            // services.AddMemoryCache();
            //Class name is not valid at this point
            // services.TryAddTransient(DbContextFactory<TDbContext>());

            // services.Add(new ServiceDescriptor(typeof(IUnitOfWork<IEfCorePersistence<TDbContext>>), provider =>
            // services.Add(new ServiceDescriptor(typeof(IUnitOfWork), provider =>
            // {
            //     var dbContextBase = provider.GetRequiredService<TDbContext>();
            //     // var uow = new UnitOfWork((new EfCorePersistence<TDbContext>(dbContextBase)));
            //     var uow = new UnitOfWork();
            //     return uow;
            // }, serviceLifetime));

            var options = new PersistenceVenderRegistrationOptions(services, typeof(TDbContext));
            options.AddDefaultRepositories();
            new EfCoreRepositoryRegistrar(options).RegistrarRepositories();
            return services;
        }
    }
}