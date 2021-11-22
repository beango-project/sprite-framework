using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Modular
{
    /// <summary>
    /// 基础模块
    /// </summary>
    public abstract class SpriteModule : ISpriteModule
    {
        public static Func<Type, bool> IsCandidate => x => x.IsClass &&
                                                           !x.IsAbstract &&
                                                           !x.IsGenericType &&
                                                           typeof(ISpriteModule).IsAssignableFrom(x);

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// 检查是否符合规约（是否为一个Sprite模块）
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentException"></exception>
        internal static void CheckIsCandidate(Type type)
        {
            if (!IsCandidate(type))
            {
                throw new ArgumentException($"The given type is not a Sprite module :{type.AssemblyQualifiedName}");
            }
        }
    }
}