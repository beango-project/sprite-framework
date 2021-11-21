using System;

namespace Sprite.UidGenerator
{
    public class UniqueIdGenerator : IUniqueIdGenerator
    {
        private readonly IUidProvider<Guid> _uidProvider;

        public UniqueIdGenerator(IUidProvider<Guid> uidProvider)
        {
            _uidProvider = uidProvider;
        }

        public Guid Create()
        {
            return _uidProvider.Create();
        }
    }
}