using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Sprite
{
    [DebuggerStepThrough]
    public static class Guard
    {
        [ContractAnnotation("value:null => halt")]
        public static T CheckNotNull<T>(T value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}