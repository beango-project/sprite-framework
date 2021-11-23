using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Core.Tests.Modular
{
    public class EmptyModule : TestModuleBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }
    }
}