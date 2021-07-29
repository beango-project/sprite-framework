using System;
using System.Collections.Generic;
using Sprite.DependencyInjection;

namespace Sprite.Data
{
    [Serializable]
    public class ConnectionStrings : Dictionary<string, string>, ISingletonInjection
    {
        public const string DefaultConnectionStringName = "Default";

        public string Default
        {
            get => TryGetValue(DefaultConnectionStringName, out var nValue) ? nValue : default;

            set => this[DefaultConnectionStringName] = value;
        }
    }
}