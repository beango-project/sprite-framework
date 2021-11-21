using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sprite.AspNetCore.Mvc.Results.Normalization
{
    public class JsonActionResultNormalizer : IActionResultNormalizer
    {
        public void Normalizer(ResultExecutingContext context)
        {
            var jsonResult =context.Result as JsonResult;
            if (jsonResult == null)
            {
                throw new ArgumentException($"{nameof(context.Result)} must be JsonResult!");
            }

            if (!(jsonResult.Value is NormalizedResultResponse))
            {
                jsonResult.Value = new RestNormalizedResultResponse(jsonResult.Value);
            }
        }
    }
}