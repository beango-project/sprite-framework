using Sprite.Web.Http;

namespace Sprite.AspNetCore.Mvc.Results
{
    public abstract class NormalizedResultResponse : INormalizedResponse
    {
        /// <summary>
        /// 访问结果是否成功
        /// 如果为 false，则需要设置 <see cref="Error" /> 信息
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误详细信息 ( 只有在 <see cref="Success" /> 为 false 时设置，而且必须设置)
        /// </summary>
        public ServiceErrorInfo Error { get; set; }
    }
}