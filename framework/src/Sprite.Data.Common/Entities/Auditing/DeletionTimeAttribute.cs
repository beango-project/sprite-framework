using System;

namespace Sprite.Data.Entities.Auditing
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,Inherited = true)]
    public class DeletionTimeAttribute: Attribute
    {
        
    }
}