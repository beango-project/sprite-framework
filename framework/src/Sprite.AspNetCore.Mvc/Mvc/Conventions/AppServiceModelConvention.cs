using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Context;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    public class AppServiceModelConvention : IApplicationModelConvention
    {
        private readonly ISpriteApplicationModelConvention _convention;

        public AppServiceModelConvention(IServiceCollection services)
        {
            _convention = services.GetSingletonInstance<ISpriteApplicationContext>()
                .ServiceProvider
                .GetRequiredService<ISpriteApplicationModelConvention>();
        }

        public void Apply(ApplicationModel application)
        {
            _convention.Apply(application);
        }
    }
}