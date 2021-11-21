using System;

namespace Sprite.UidGenerator.Providers
{
    public class GuidProvider : IGuidProvider
    {
        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}