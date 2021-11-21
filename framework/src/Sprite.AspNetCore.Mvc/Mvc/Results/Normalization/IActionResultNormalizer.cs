using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sprite.AspNetCore.Mvc.Results.Normalization
{
    /// <summary>
    /// <see cref="IActionResult" />规范器，用于规范化返回结果
    /// </summary>
    public interface IActionResultNormalizer
    {
        /// <summary>
        /// 规范化
        /// </summary>
        /// <param name="ResultExecutingContext">conext</param>
        void Normalizer(ResultExecutingContext context);
    }
}