using System.Linq;
using System.Linq.Expressions;
using ImTools;

namespace System.Reflection
{
    public static class PropertyHelper
    {
        // public static  ImHashMap<Type, PropertyInfo> CachedObjectProperties = 

        public static void TrySetProperty<TObject, TValue>(
            TObject obj,
            Func<TObject, TValue> propertySelector,
            Func<TValue> valueFactory,
            params Type[] ignoreAttributeTypes)
        {
     
            // TrySetProperty(obj, propertySelector, x => valueFactory(), ignoreAttributeTypes);
        }
    }
    
}