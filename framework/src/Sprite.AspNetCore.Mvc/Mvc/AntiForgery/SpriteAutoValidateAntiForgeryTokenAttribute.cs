using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SpriteAutoValidateAntiForgeryTokenAttribute : Attribute, IFilterFactory, IFilterMetadata, IOrderedFilter
    {
        public bool IsReusable { get; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<SpriteAutoValidateAntiforgeryTokenAuthorizationFilter>();
        }

        /// <summary>
        /// </summary>
        public int Order { get; } = 1000;
    }
}