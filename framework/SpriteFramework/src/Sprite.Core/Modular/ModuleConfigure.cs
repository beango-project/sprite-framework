using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Modular
{
    public abstract class ModuleConfigure
    {
        [NotNull] public Type[] DependedModules { get; private set; }

        protected void ImportModules(params Type[] dependedModules)
        {
            DependedModules = dependedModules ?? new Type[0];
        }

        protected bool SkipScanner = false;


        public abstract void Configure();
    }
}