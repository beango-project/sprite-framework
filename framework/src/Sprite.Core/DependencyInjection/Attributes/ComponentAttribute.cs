using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        /// <summary>
        /// Service life cycle<see cref="ServiceLifetime"/>
        /// </summary>
        public virtual ServiceLifetime? Scope { get; set; }

        /// <summary>
        /// Try to register.
        /// </summary>
        public virtual bool TryRegister { get; set; }

        /// <summary>
        /// Setting this will replace the previously registered service .
        /// </summary>
        public virtual bool Replace { get; set; }

        public ComponentAttribute()
        {
        }


        public ComponentAttribute(ServiceLifetime scope)
        {
            this.Scope = scope;
        }
    }
}