using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sprite.AspNetCore.Mvc.Results.Normalization
{
    public class ObjectResultNormalizer : IActionResultNormalizer
    {
        private readonly IServiceProvider _serviceProvider;

        public ObjectResultNormalizer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Normalizer(ResultExecutingContext context)
        {
            var objectResult = context.Result as ObjectResult;
            if (objectResult == null)
            {
                throw new ArgumentException($"{nameof(context.Result)} must be ObjectResult!");
            }


            objectResult.DeclaredType = typeof(RestNormalizedResultResponse);
            objectResult.Value = new RestNormalizedResultResponse(objectResult.Value);
            
            // //TODO support Newtonsoft.Json ?
            // if (!objectResult.Formatters.Any(f => f is SystemTextJsonOutputFormatter || f is NewtonsoftJsonOutputFormatter))
            // {
            //     //获取选项配置的json序列号配置选项，加入obj返回值的格式化选项
            //     var jsonSerializerOptiodns = _serviceProvider.GetService<IOptions<JsonOptions>>()?.Value.JsonSerializerOptions;
            //     var jsonSerializerOptions = _serviceProvider.GetService<IOptions<JsonSerializerOptions>>()?.Value;
            //     objectResult.Formatters.Add(new SystemTextJsonOutputFormatter(jsonSerializerOptions));
            //     objectResult.Formatters.Add(new SystemTextJsonOutputFormatter(jsonSerializerOptiodns));
            // }
        }
    }
}