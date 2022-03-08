using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.Tests;
using Xunit;

namespace Sprite.Data.Common.Tests
{
    public class ConnectionStringResolver_Tests : SpriteIntegratedTest<DataCommonTestModule>
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public ConnectionStringResolver_Tests()
        {
            _connectionStringResolver = ServiceProvider.GetRequiredService<IConnectionStringResolver>();
        }

        [Fact]
        public void Should_Get_Defined_If_Defined()
        {
           _connectionStringResolver.GetConnectionString(DataCommonTestModule.Db1).ShouldBe(DataCommonTestModule.Db1ConnStr);
        }
        
        [Fact]
        public void  Should_Get_Default_If_Not_Specified()
        {
            _connectionStringResolver.GetConnectionString().ShouldBe(DataCommonTestModule.DefaultConnStr);
        }

        [Fact]
        public void Should_Get_Default_If_Give_Not_Found()
        {
            _connectionStringResolver.GetConnectionString(DataCommonTestModule.Db2).ShouldBe(DataCommonTestModule.DefaultConnStr);
        }
    }
}