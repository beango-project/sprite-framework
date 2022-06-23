using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Data.Ado;
using Sprite.Data.Repositories;
using Sprite.Data.Tests.Persistence;
using Sprite.Modular;

namespace Sprite.Data.Tests
{
    class TestModuleConfig : ModuleConfig
    {
        public override Type[] ImportModules()
        {
            return new[] { typeof(SpriteDataModule) };
        }


        public override void Configure()
        {
        }
    }

    [Usage(typeof(TestModuleConfig))]
    public class DataTestModule : SpriteModule
    {
        private const string ConnectionStrings = "Data Source=:memory:";

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbConnectionOptions>(options => options.ConnectionStrings.Default = ConnectionStrings);
            services.Replace(ServiceDescriptor.Singleton<IConfiguration>(_ =>
                new ConfigurationBuilder().Build()
            ));
            var connection = CreateConnectionAndDB();
            services.AddTransient<AdoSqlite>(_ => new AdoSqlite(connection));
            services.AddTransient<Ado<SqliteConnection>, AdoSqlite>(sp => sp.GetRequiredService<AdoSqlite>());
        }

        private SqliteConnection CreateConnectionAndDB()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            return connection;
        }
    }
}