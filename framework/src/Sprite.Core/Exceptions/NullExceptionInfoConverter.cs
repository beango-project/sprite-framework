using System;

namespace Sprite.Exceptions
{
    public class NullExceptionInfoConverter : IExceptionInfoConverter
    {
        public T Convert<T>(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}