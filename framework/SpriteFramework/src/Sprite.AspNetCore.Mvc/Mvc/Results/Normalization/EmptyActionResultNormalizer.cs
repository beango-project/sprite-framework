using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sprite.AspNetCore.Mvc.Results.Normalization
{
    /// <summary>
    /// Default Wrapper <see cref="ObjectResult" />
    /// </summary>
    public class EmptyActionResultNormalizer : IActionResultNormalizer
    {
        public void Normalizer(ResultExecutingContext context)
        {
            context.Result = new ObjectResult(new RestNormalizedResultResponse());
        }
    }
}