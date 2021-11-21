using System.Collections.Generic;

namespace Sprite.MultiTenancy
{
    public class MultiTenancyOptions
    {
        public List<ITenantResolutionStrategy> ResolutionStrategies { get; }

        public MultiTenancyOptions()
        {
            ResolutionStrategies = new List<ITenantResolutionStrategy>();
        }
    }
}