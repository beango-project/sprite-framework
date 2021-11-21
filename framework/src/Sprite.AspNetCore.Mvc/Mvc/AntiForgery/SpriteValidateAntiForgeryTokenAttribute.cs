using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SpriteValidateAntiForgeryTokenAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        /// <inheritdoc />
        public bool IsReusable => true;

        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<SpriteValidateAntiforgeryTokenAuthorizationFilter>();
        }

        /// <summary>
        /// Look at <see cref="IOrderedFilter.Order" /> for more detailed info.
        /// </summary>
        public int Order { get; set; } = 1000;
    }
}