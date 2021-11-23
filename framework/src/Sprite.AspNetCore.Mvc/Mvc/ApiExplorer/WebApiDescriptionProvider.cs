using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.AspNetCore.Mvc.Abstractions;
using Sprite.DependencyInjection.Attributes;
using Sprite.Remote;

namespace Sprite.AspNetCore.Mvc.ApiExplorer
{
    /// <summary>
    /// Web(Http)ApiDescriptionProvider
    /// 用于处理框架自定义Action的返回
    /// </summary>
    [Component(ServiceLifetime.Transient)]
    [Export(typeof(IApiDescriptionProvider), typeof(IHybridApiDescriptionProvider))]
    public class WebApiDescriptionProvider : IHybridApiDescriptionProvider
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly MvcOptions _mvcOptions;
        private readonly RemoteApiDescriptionProviderOptions _options;

        public WebApiDescriptionProvider(IOptions<MvcOptions> mvcOptionsAccessor, IModelMetadataProvider modelMetadataProvider,
            IOptions<RemoteApiDescriptionProviderOptions> optionsAccessor)
        {
            _mvcOptions = mvcOptionsAccessor.Value;
            _modelMetadataProvider = modelMetadataProvider;
            _options = optionsAccessor.Value;
        }

        public int Order => -999;

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            foreach (var apiResponseType in GetApiResponseTypes())
            {
                foreach (var apiResult in context.Results.Where(IsCandidate))
                {
                    if (apiResult.ActionDescriptor is not ControllerActionDescriptor)
                    {
                        throw new Exception($"{nameof(apiResult.ActionDescriptor)}  should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
                    }

                    var methodInfo = apiResult.ActionDescriptor.AsControllerActionDescriptor().MethodInfo;
                    var attributes = ReflectionHelper.GetAttributesOfMemberInfo<ProducesResponseTypeAttribute>(methodInfo);
                    if (attributes.Any(x => x.StatusCode == apiResponseType.StatusCode))
                    {
                        continue;
                    }

                    apiResult.SupportedResponseTypes.AddIfNotContains(x => x.StatusCode == apiResponseType.StatusCode, () => apiResponseType);
                }
            }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
        }


        protected virtual List<ApiResponseType> GetApiResponseTypes()
        {
            foreach (var apiResponse in _options.SupportedResponseTypes)
            {
                //获取响应类型的元数据模型
                apiResponse.ModelMetadata = _modelMetadataProvider.GetMetadataForType(apiResponse.Type);
                var typeMetadataProviders = _mvcOptions.OutputFormatters.OfType<IApiResponseTypeMetadataProvider>();
                foreach (var responseTypeMetadataProvider in typeMetadataProviders)
                {
                    {
                        var formatterSupportedContentTypes = responseTypeMetadataProvider.GetSupportedContentTypes(null, apiResponse.Type);
                        if (formatterSupportedContentTypes == null)
                        {
                            continue;
                        }

                        foreach (var formatterSupportedContentType in formatterSupportedContentTypes)
                        {
                            apiResponse.ApiResponseFormats.Add(new ApiResponseFormat
                            {
                                Formatter = (IOutputFormatter) responseTypeMetadataProvider,
                                MediaType = formatterSupportedContentType
                            });
                        }
                    }
                }
            }

            return _options.SupportedResponseTypes.ToList();
        }

        protected virtual bool IsCandidate(ApiDescription actionDescriptor)
        {
            if (actionDescriptor.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                if (typeof(IRemoteService).IsAssignableFrom(controllerActionDescriptor.ControllerTypeInfo))
                {
                    return true;
                }
            }

            return false;
        }
    }
}