using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Data.EntityFrameworkCore.Uow;
using Sprite.Modular;

namespace Sprite.Data.EntityFrameworkCore
{
    [Usage(typeof(SpriteEfCoreModuleConfigure))]
    public class SpriteEfCoreModule : Module
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.TryAddTransient(typeof(IDbContextProvider<>),typeof(UnitOfWorkPersistenceVenderProvider<>));
        }
    }
}