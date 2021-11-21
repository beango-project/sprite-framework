using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sprite.AspNetCore.Mvc.Results.Normalization
{
    public class NullActionResultNormalizer : IActionResultNormalizer
    {
        public void Normalizer(ResultExecutingContext context)
        {
        }
    }
}