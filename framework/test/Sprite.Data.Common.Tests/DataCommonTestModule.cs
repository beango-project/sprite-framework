using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Modular;

namespace Sprite.Data.Common.Tests
{
    public class DataCommonTestModule : SpriteModule
    {
        public const string DefaultConnStr = "Default";
        public const string Db1 = "Database1";
        public const string Db1ConnStr = "Db1_Conn";
        public const string Db2 = "Db2";

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = DefaultConnStr;
                options.ConnectionStrings[Db1] = Db1ConnStr;
            });
            services.TryAddTransient<IConnectionStringResolver, ConnectionStringResolver>();
        }
    }
}