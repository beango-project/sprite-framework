using System;
using Sprite.UidGenerator;

namespace Sprite.UidGenerator
{
    public class UniqueIdGeneratorOptions
    {
        public UniqueIdGeneratorType UniqueIdGeneratorType { get; set; }

        internal bool IsDistributed { get;  set; }
    }
}