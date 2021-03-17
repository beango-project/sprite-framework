using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Modular
{
    /// <summary>
    /// 基础模块
    /// </summary>
    public abstract class Module : IModule
    {
        public void ConfigureServices(IServiceCollection service)
        {
        }

        public void Configure(OnApplicationInitContext context)
        {
        }
    }
}