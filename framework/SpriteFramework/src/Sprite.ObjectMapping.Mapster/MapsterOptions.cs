using System.Collections.Generic;
using System.Reflection;
using FastExpressionCompiler;
using Mapster;
using Sprite.ObjectMapping.Mapster.Attributes;

namespace Sprite.ObjectMapping.Mapster
{
    public class MapsterOptions
    {
        public MapsterOptions()
        {
            Registers = new List<IRegister>();
            Config.Default.IgnoreAttribute(typeof(NotMapAttribute));
            Config.Compiler = expression => expression.CompileFast();
        }

        public TypeAdapterConfig Config => TypeAdapterConfig.GlobalSettings;

        public List<IRegister> Registers { get; }

        public void AddMaps(Assembly assembly)
        {
            var registers = Config.Scan(assembly);
            foreach (var register in registers)
            {
                Registers.AddIfNotContains(register);
            }
        }

        public void AddMaps<TModule>()
        {
            AddMaps(typeof(TModule).Assembly);
        }


        public void AddRegister(IRegister register)
        {
            Config.Apply(register);
        }
    }
}