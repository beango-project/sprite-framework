using System;

namespace Sprite.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method| AttributeTargets.Interface)]
    public class NonNormalizeAttribute: Attribute
    {
        
    }
}