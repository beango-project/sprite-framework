using System.Collections.Generic;
using ImTools;

namespace Sprite.Common
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