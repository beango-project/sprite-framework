using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Sprite
{
    [DebuggerStepThrough]
    public static class Check
    {
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(T value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotNullOrEmpty(string value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if(value.IsNullOrEmpty())
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}