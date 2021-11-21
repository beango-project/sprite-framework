using System;
using System.Collections.Generic;

namespace Sprite.Data.Uow
{
    public class ScopedUnitOfWorkStorage
    {
        public IList<IUnitOfWork> UnitOfWorks { get; }

        public IList<IUnitOfWork> ActualUnitOfWorks { get; }

        public ScopedUnitOfWorkStorage()
        {
            UnitOfWorks = new List<IUnitOfWork>();
            ActualUnitOfWorks = new List<IUnitOfWork>();
        }
    }
}