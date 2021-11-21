using System;

namespace Sprite.Data.Entities.Auditing
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LastModifiedDateAttribute : Attribute
    {
    }
}