using System.Collections.Generic;
using System.Reflection;
using Mapster;

namespace Sprite.ObjectMapping.Mapster
{
    public class MapsterOptions
    {
        public MapsterOptions()
        {
            Registers = new List<IRegister>();
        }

        public TypeAdapterConfig GlobalSettings => TypeAdapterConfig.GlobalSettings;
        public List<IRegister> Registers { get; }

        public void AddMaps(Assembly assembly)
        {
            var registers = GlobalSettings.Scan(assembly);
            foreach (var register in registers)
            {
                Registers.AddIfNotContains(register);
            }

            GlobalSettings.Apply(registers);
        }

        public void AddMaps<TModule>()
        {
            AddMaps(typeof(TModule).Assembly);
        }


        public void AddRegister(IRegister register)
        {
            GlobalSettings.Apply(register);
        }
    }
}