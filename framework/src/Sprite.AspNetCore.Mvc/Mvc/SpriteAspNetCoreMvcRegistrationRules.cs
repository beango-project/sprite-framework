using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection;

namespace Sprite.AspNetCore.Mvc
{
    public class SpriteAspNetCoreMvcRegistrationRules : DefaultRegistrationRules
    {
        public override void AddFromTypeOf(IServiceCollection services, Type type)
        {
            if (!IsMvcComponent(type))
            {
                return;
            }

            var lifeTime = ServiceLifetime.Transient;
            var serviceTypes = ExportServiceScanner.GetServices(type);

            foreach (var serviceType in serviceTypes)
            {
                services.Add(ServiceDescriptor.Describe(serviceType, type, lifeTime));
            }
        }

        /// <summary>
        /// 是否为Mvc的组件
        /// <see cref="Controller" />
        /// <see cref="ViewComponent" />
        /// <see cref="PageModel" />
        /// 或者声明定义的对应类型的Attribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual bool IsMvcComponent(Type type)
        {
            return IsController(type) || IsPageModel(type) || IsViewComponent(type);
        }

        private static bool IsController(Type type)
        {
            return typeof(Controller).IsAssignableFrom(type) || type.IsDefined(typeof(ControllerAttribute), true);
        }

        private static bool IsPageModel(Type type)
        {
            return typeof(PageModel).IsAssignableFrom(type) || type.IsDefined(typeof(PageModelAttribute), true);
        }

        private static bool IsViewComponent(Type type)
        {
            return typeof(ViewComponent).IsAssignableFrom(type) || type.IsDefined(typeof(ViewComponentAttribute), true);
        }
    }
}