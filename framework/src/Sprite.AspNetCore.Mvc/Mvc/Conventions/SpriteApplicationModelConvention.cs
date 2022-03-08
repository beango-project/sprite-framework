using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImmediateReflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection;
using Sprite.Http;
using Sprite.Remote;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    public class SpriteApplicationModelConvention : ISpriteApplicationModelConvention, ITransientInjection
    {
        public SpriteApplicationModelConvention(IOptions<AspNetCoreMvcOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AspNetCoreMvcOptions Options { get; }

        public void Apply(ApplicationModel application)
        {
            ApplyControllers(application.Controllers);
        }

        /// <summary>
        /// 应用控制器
        /// </summary>
        /// <param name="applicationControllers">用于配置控制器的模型</param>
        protected virtual void ApplyControllers(IList<ControllerModel> controllerModels)
        {
            foreach (var controller in controllerModels)
            {
                var controllerType = controller.ControllerType.AsType();

                var description = GetControllerDescription(controllerType);
                if (typeof(IRemoteService).GetTypeInfo().IsAssignableFrom(controllerType))
                {
                    controller.ControllerName = controller.ControllerName.RemoveSuffix(HttpUtils.CommonPostfixes);
                    description?.ControllerOptions?.Invoke(controller);
                    ConfigureController(controller, description);
                }
            }
        }

        private void RemoveDuplicateControllers(IList<ControllerModel> controllerModels)
        {
            // var removeControllerModels = new List<ControllerModel>();
            // foreach (var controllerModel in controllerModels)
            // {
            //     if (!controllerModel.ControllerType.IsDefined(typeof(ExportAttribute), false))
            //     {
            //         continue;
            //     }
            //
            //     var exportAttribute = controllerModel.ControllerType.GetAttributeWithDefined<ExportAttribute>()
            //     if (exportAttribute != null && exportAttribute.IncludeSelf)
            //     {
            //         var exportControllerModels = controllerModels.Where(m => exportAttribute.ServiceTypes
            //             .Contains(m.ControllerType));
            //         removeControllerModels.AddRange(exportControllerModels);
            //         continue;
            //     }
            //     var baseControllerTypes=controllerModel.ControllerType.geba
            // }
        }

        /// <summary>
        /// 配置控制器，
        /// </summary>
        /// <param name="controller">用于配置控制器的模型</param>
        /// <param name="description">控制器规约说明</param>
        private void ConfigureController(ControllerModel controller, ConventionControllerDescription description)
        {
            ConfigureApiExplorer(controller);
            ConfigureSelector(controller, description);
            ConfigureParameters(controller);
        }

        private void ConfigureParameters(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                foreach (var para in action.Parameters)
                {
                    if (para.BindingInfo != null)
                    {
                        continue;
                    }

                    var parameterType = para.ParameterInfo.ParameterType;

                    if (TypeHelper.IsTypeOfDisplayLiteralExpression(parameterType))
                    {
                        if (CanUseFormBodyBinding(action, para))
                        {
                            para.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                        }
                    }
                    else if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && parameterType.GenericTypeArguments.Any())
                    {
                        if (TypeHelper.IsTypeOfDisplayLiteralExpression(parameterType.GenericTypeArguments[0]))
                        {
                            if (CanUseFormBodyBinding(action, para))
                            {
                                para.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                            }
                        }
                    }
                }
            }
        }

        protected virtual void ConfigureSelector(ControllerModel controller, ConventionControllerDescription description)
        {
            FilterEmptySelector(controller.Selectors);

            var controllerType = controller.ControllerType.AsType();
            var remoteServiceAttribute = controllerType.GetAttributeWithDefined<RemoteServiceAttribute>();
            if (remoteServiceAttribute != null && !remoteServiceAttribute.IsEnabled)
            {
                return;
            }

            if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
            {
                return;
            }

            var rootPath = GetRootPathOrDefault(controllerType);

            foreach (var action in controller.Actions)
            {
                ConfigureSelector(rootPath, controller.ControllerName, action, description);
            }
        }

        protected virtual void ConfigureSelector(string rootPath, string controllerName, ActionModel action, [CanBeNull] ConventionControllerDescription description)
        {
            FilterEmptySelector(action.Selectors);

            var remoteServiceAtt = action.ActionMethod.GetAttributeWithDefined<RemoteServiceAttribute>();
            if (remoteServiceAtt != null && !remoteServiceAtt.IsEnabled)
            {
                return;
            }

            if (!action.Selectors.Any())
            {
                AddServiceSelector(rootPath, controllerName, action, description);
            }
            else
            {
                NormalizeSelectorRoutes(rootPath, controllerName, action, description);
            }
        }

        protected virtual void NormalizeSelectorRoutes(string rootPath, string controllerName, ActionModel action, ConventionControllerDescription description)
        {
            foreach (var selector in action.Selectors)
            {
                var httpMethod = selector.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods?.FirstOrDefault();

                if (httpMethod == null)
                {
                    httpMethod = SelectHttpMethod(action, description);
                }

                if (selector.AttributeRouteModel == null)
                {
                    selector.AttributeRouteModel = CreateAttributeRouteModel(rootPath, controllerName, action, httpMethod, description);
                }

                if (!selector.ActionConstraints.OfType<HttpMethodActionConstraint>().Any())
                {
                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));
                }
            }
        }

        protected virtual void AddServiceSelector(string rootPath, string controllerName, ActionModel action, [CanBeNull] ConventionControllerDescription description)
        {
            var httpMethod = SelectHttpMethod(action, description);

            var serviceSelectorModel = new SelectorModel
            {
                AttributeRouteModel = CreateAttributeRouteModel(rootPath, controllerName, action, httpMethod, description),
                ActionConstraints = { new HttpMethodActionConstraint(new[] { httpMethod }) }
            };

            action.Selectors.Add(serviceSelectorModel);
        }

        /// <summary>
        /// 创建路由特性模型
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="controllerName"></param>
        /// <param name="action"></param>
        /// <param name="httpMethod"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private AttributeRouteModel CreateAttributeRouteModel(string rootPath, string controllerName, ActionModel action, string httpMethod,
            ConventionControllerDescription description)
        {
            var route = new ConventionalRouterBuilder(rootPath, controllerName, action, httpMethod, description).Build();
            return new AttributeRouteModel(new RouteAttribute(route));
        }

        protected virtual string SelectHttpMethod(ActionModel action, ConventionControllerDescription description)
        {
            return HttpUtils.ConvertHttpMethod(action.ActionName);
        }

        /// <summary>
        /// 过滤空的选择器
        /// </summary>
        /// <param name="selectors">选择器</param>
        private void FilterEmptySelector(IList<SelectorModel> selectors)
        {
            selectors.Where(IsEmptySelector).ToList().ForEach(x => selectors.Remove(x));
        }

        protected virtual bool IsEmptySelector(SelectorModel selector)
        {
            return selector.AttributeRouteModel == null
                   && (selector.ActionConstraints is null || selector.ActionConstraints.Count <= 0)
                   && (selector.EndpointMetadata is null || selector.EndpointMetadata.Count <= 0);
        }

        /// <summary>
        /// 获取根路径,如果为跟路径空则从<see cref="AreaAttribute" />获取，默认为:services
        /// </summary>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        protected virtual string GetRootPathOrDefault(Type controllerType)
        {
            var description = GetControllerDescription(controllerType);
            if (description?.RootPath != null)
            {
                return description.RootPath;
            }

            var areaAttribute = controllerType.GetImmediateType().GetAllAttributes().OfType<AreaAttribute>().FirstOrDefault();
            if (areaAttribute?.RouteValue != null)
            {
                return areaAttribute.RouteValue;
            }

            return "services";
        }

        protected virtual void ConfigureApiExplorer(ControllerModel controller)
        {
            if (controller.ApiExplorer.GroupName.IsNullOrEmpty())
            {
                controller.ApiExplorer.GroupName = controller.ControllerName;
            }

            if (controller.ApiExplorer.IsVisible == null)
            {
                controller.ApiExplorer.IsVisible = IsVisibleRemoteService(controller.ControllerType);
            }

            foreach (var action in controller.Actions)
            {
                ConfigureApiExplorer(action);
            }
        }

        protected virtual void ConfigureApiExplorer(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible != null)
            {
                return;
            }

            var visible = IsVisibleRemoteServiceMethod(action.ActionMethod);
            if (visible == null)
            {
                return;
            }

            action.ApiExplorer.IsVisible = visible;
        }

        protected virtual bool? IsVisibleRemoteService(TypeInfo controllerTypeInfo)
        {
            var remoteAttribute = controllerTypeInfo.GetAttributeWithDefined<RemoteServiceAttribute>();
            if (remoteAttribute == null)
            {
                return true;
            }

            return remoteAttribute.IsEnabled;
        }

        protected virtual bool? IsVisibleRemoteServiceMethod(MethodInfo actionActionMethod)
        {
            var remoteAttribute = actionActionMethod.GetAttributeWithDefined<RemoteServiceAttribute>();
            if (remoteAttribute == null)
            {
                return null;
            }

            return remoteAttribute.IsEnabled;
        }

        private ConventionControllerDescription GetControllerDescription(Type controllerType)
        {
            return Options.Controllers.ControllerDescriptions.FirstOrDefault(t => t.ControllerTypes.Contains(controllerType));
        }

        protected virtual bool CanUseFormBodyBinding(ActionModel action, ParameterModel parameter)
        {
            if (Options.Controllers.FormBodyBindingIgnoredTypes.Any(t => t.IsAssignableFrom(parameter.ParameterInfo.ParameterType)))
            {
                return false;
            }

            foreach (var selector in action.Selectors)
            {
                if (selector.ActionConstraints == null)
                {
                    continue;
                }

                foreach (var actionConstraint in selector.ActionConstraints)
                {
                    var httpMethodActionConstraint = actionConstraint as HttpMethodActionConstraint;
                    if (httpMethodActionConstraint == null)
                    {
                        continue;
                    }

                    if (httpMethodActionConstraint.HttpMethods.All(hm => hm.IsIn("GET", "DELETE", "TRACE", "HEAD")))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}