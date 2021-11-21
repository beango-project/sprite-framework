using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    public class ConventionalControllerOptions
    {
        public ConventionalControllerOptions()
        {
            ControllerDescriptions = new List<ConventionControllerDescription>();
            FormBodyBindingIgnoredTypes = new List<Type>();
        }

        public List<ConventionControllerDescription> ControllerDescriptions { get; }

        /// <summary>
        /// Ignore MVC Form Binding types.
        /// </summary>
        public List<Type> FormBodyBindingIgnoredTypes { get; }

        public ConventionalControllerOptions Add(Type type, Action<ConventionControllerDescription> action = null)
        {
            return Add(type.Assembly, action);
        }

        public ConventionalControllerOptions Add(Assembly assembly, Action<ConventionControllerDescription> action = null)
        {
            var description = new ConventionControllerDescription(assembly, "services", "Default");
            action?.Invoke(description);
            description.Load();
            ControllerDescriptions.Add(description);
            return this;
        }
    }
}