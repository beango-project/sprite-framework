using System;
using Sprite.Remote;

namespace Sprite.Exceptions
{
    public static class ExceptionConvertExtensions
    {
        /// <summary>
        ///     转换为错误信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ServiceErrorInfo ConvertErrorInfo(this Exception exception)
        {
            return CreateErrorInfo(exception);
        }

        public static ServiceErrorInfo CreateErrorInfo(Exception exception)
        {
            // if (exception is UserFriendlyException)
            // {
            //     var userFriendlyException = exception as UserFriendlyException;
            //     return new ServiceErrorInfo(userFriendlyException.Message, userFriendlyException.Details);
            // }

            // if (exception is CtcsValidationException)
            // {
            //     var ctcsValidationException = exception as CtcsValidationException;
            //     return new ServiceErrorInfo(ctcsValidationException.Message, ctcsValidationException.Errors.ToString());
            // }

            return new ServiceErrorInfo(exception.Message);
        }
    }
}