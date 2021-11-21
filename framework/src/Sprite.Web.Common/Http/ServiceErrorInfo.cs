namespace Sprite.Web.Http
{
    public class ServiceErrorInfo
    {
        public ServiceErrorInfo(string code, string message = null, string details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        /// <summary>
        /// Error code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error details.
        /// </summary>
        public string Details { get; set; }
    }
}