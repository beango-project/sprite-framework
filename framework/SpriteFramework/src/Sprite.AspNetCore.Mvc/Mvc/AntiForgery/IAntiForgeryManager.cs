using Microsoft.AspNetCore.Http;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    /// <summary>
    /// 防伪管理器
    /// </summary>
    public interface IAntiForgeryManager
    {
        /// <summary>
        /// 防伪选项
        /// </summary>
        SpriteAntiforgeryOptions Options { get; }

        HttpContext HttpContext { get; }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        void SetCookie();

        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <returns>令牌串</returns>
        string GenerateToken();
    }
}