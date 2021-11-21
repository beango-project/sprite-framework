using System.Collections.Generic;

namespace Sprite.DynamicProxy
{
    public class InvocationOptions
    {
        public InvocationOptions()
        {
            Interceptors = new List<IAspectInterceptor>();
        }

        public List<IAspectInterceptor> Interceptors { get; }
    }
}