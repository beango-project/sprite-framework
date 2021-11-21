using System.Reflection;
using Sprite.DependencyInjection.Attributes;
using Stashbox.Configuration;

namespace Sprite.DependencyInjection.Stashbox
{
    public static class SpriteStashboxConfiguration
    {
        /// <summary>
        /// 自动成员注入规则
        /// </summary>
        /// <returns></returns>
        public static Rules.AutoMemberInjectionRules AutoMemberInjectionRule =>
            Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess |
            Rules.AutoMemberInjectionRules.PrivateFields |
            Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter;


        /// <summary>
        /// 使用Sprite的依赖注入容器配置，自动处理解决循环依赖
        /// 类内部的成员注入规则使用<see cref="AutoMemberInjectionRule" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static ContainerConfigurator WithSpriteInjection(this ContainerConfigurator configurator)
        {
            configurator.WithCircularDependencyWithLazy()
                .WithAutoMemberInjection(AutoMemberInjectionRule, x =>
                {
                    if (x.IsDefined(typeof(AutowiredAttribute)))
                    {
                        return true;
                    }

                    return false;
                });
            return configurator;
        }
    }
}