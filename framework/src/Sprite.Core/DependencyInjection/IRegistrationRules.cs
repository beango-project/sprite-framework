using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.DependencyInjection
{
    /// <summary>
    /// 注册规则
    /// </summary>
    public interface IRegistrationRules
    {
        void AddFromAssemblyOf(IServiceCollection services, Assembly assembly);

        void AddFromTypesOf(IServiceCollection services, Type[] types);

        void AddFromTypeOf(IServiceCollection services, Type type);
    }
}