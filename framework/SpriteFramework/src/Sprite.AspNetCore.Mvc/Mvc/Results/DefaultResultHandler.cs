using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Sprite.AspNetCore.Mvc.Results.Normalization;

namespace Sprite.AspNetCore.Mvc.Results
{
    public class DefaultResultHandler : ResultHandler
    {
        private IActionResultNormalizer resultNormalizer;

        public override void Handle(ResultExecutingContext context)
        {
            Check.NotNull(context, nameof(context));

            GetNormalizerKind(context).Normalizer(context);
        }

        protected virtual IActionResultNormalizer GetNormalizerKind(ResultExecutingContext context)
        {
            if (context.Result is ContentResult)
            {
                
            }
            if (context.Result is ObjectResult)
            {
                return new ObjectResultNormalizer(context.HttpContext.RequestServices);
            }

            if (context.Result is JsonResult)
            {
                return new JsonActionResultNormalizer();
            }

            if (context.Result is EmptyResult)
            {
                return new EmptyActionResultNormalizer();
            }

            return new NullActionResultNormalizer();
        }
    }
}