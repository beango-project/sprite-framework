using System;

namespace Sprite.Remote
{
    [Serializable]
    public class ServiceErrorInfo
    {
        public ServiceErrorInfo()
        {
        }


        public ServiceErrorInfo(string message, string details = null, string code = null)
        {
            Message = message;
            Details = details;
            Code = code;
        }

        /// <summary>
        ///     Error code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Error details.
        /// </summary>
        public string Details { get; set; }
    }
}