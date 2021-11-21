using System;

namespace Sprite.Exceptions
{
    /// <summary>
    /// 异常信息转换器
    /// </summary>
    public interface IExceptionInfoConverter
    {
        T Convert<T>(Exception exception);
    }
}