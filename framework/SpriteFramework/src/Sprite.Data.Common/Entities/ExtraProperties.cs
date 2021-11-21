using System.Collections.Generic;
using ImTools;

namespace Sprite.Data.Entities
{
    public class ExtraProperties : Dictionary<string, object>
    {
        public ExtraProperties()
        {
        }

        public ExtraProperties(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }
    }
}