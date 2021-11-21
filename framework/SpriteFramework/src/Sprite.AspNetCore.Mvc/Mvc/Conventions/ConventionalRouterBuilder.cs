using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Sprite.Http;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    public class ConventionalRouterBuilder : IConventionalRouterBuilder
    {
        public ConventionalRouterBuilder(string rootPath, string controllerName, ActionModel action, string httpMethod, ConventionControllerDescription description)
        {
            RootPath = rootPath;
            ControllerName = controllerName;
            Action = action;
            HttpMethod = httpMethod;
            Description = description;
        }

        public IList<string> Routers { get; }

        public ConventionControllerDescription Description { get; }
        public string RootPath { get; }
        public string ControllerName { get; }
        public ActionModel Action { get; }
        public string HttpMethod { get; }


        public string Build()
        {
            var controllerNameInUrl = NormalizeControllerNameInUrl(RootPath, ControllerName, Description);

            var url = $"api/{RootPath}/" + NormalizeControllerNameCase(ControllerName, Description);

            // var parameters = Action.Parameters;
            //
            // //处理id
            // var idParameterModel = parameters.FirstOrDefault(p => p.ParameterName == "id");
            // if (idParameterModel != null)
            // {
            //     if (TypeHelper.IsTypeOfDisplayLiteralExpression(idParameterModel.ParameterType))
            //     {
            //         url += "/{id}";
            //     }
            //     // else
            //     // {
            //     //     var properties = idParameterModel
            //     //         .ParameterType
            //     //         .GetProperties(BindingFlags.Instance | BindingFlags.Public);
            //     //
            //     //     foreach (var property in properties)
            //     //     {
            //     //         url += "/{" + property.Name + "}";
            //     //     }
            //     // }
            // }

            var actionNameInUrl = NormalizeActionNameInUrl(RootPath, ControllerName, Action, HttpMethod, Description);
            if (!actionNameInUrl.IsNullOrEmpty())
            {
                url += $"/{NormalizeActionNameCase(actionNameInUrl, Description)}";
            }

            return url;
        }

        protected virtual string NormalizeActionNameCase(string actionNameInUrl, ConventionControllerDescription description)
        {
            return actionNameInUrl.ToCamelCase();
        }

        protected virtual string NormalizeControllerNameInUrl(string rootPath, string controllerName, [CanBeNull] ConventionControllerDescription description)
        {
            if (description?.ControllerRouteMap is null)
            {
                return controllerName;
            }

            return description.ControllerRouteMap(new ControllerRouterGuide(rootPath, controllerName));
        }

        protected virtual string NormalizeControllerNameCase(string controllerName, [CanBeNull] ConventionControllerDescription description)
        {
            return controllerName.ToCamelCase();
        }

        protected virtual string NormalizeActionNameInUrl(string rootPath, string controllerName, ActionModel action, string httpMethod,
            [CanBeNull] ConventionControllerDescription description)
        {
            var actionName = HttpUtils.RemoveHttpMethodPrefix(action.ActionName, httpMethod).RemoveSuffix("Async");
            if (description?.ActionRouteMap == null)
            {
                return actionName;
            }

            return null;
        }
    }
}