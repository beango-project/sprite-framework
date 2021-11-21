using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Sprite.AspNetCore.Mvc.ApiExplorer
{
    /// <summary>
    /// 远程调用Api描述提供器配置选项
    /// </summary>
    public class RemoteApiDescriptionProviderOptions
    {
        /// <summary>
        /// 获取响应的可能格式的列表。
        /// </summary>
        public HashSet<ApiResponseType> SupportedResponseTypes { get; set; } = new();
    }
}