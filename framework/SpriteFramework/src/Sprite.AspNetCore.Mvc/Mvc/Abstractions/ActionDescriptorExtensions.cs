using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ImTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sprite.AspNetCore.Mvc.Abstractions
{
    public static class ActionDescriptorExtensions
    {
        public static List<Type> ObjectResultTypes = new List<Type>
        {
            typeof(JsonResult),
            typeof(ObjectResult),
            typeof(NoContentResult)
        };


        public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor is ControllerActionDescriptor;
        }

        public static bool IsPageAction(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor is PageActionDescriptor;
        }


        public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
        {
            if (!actionDescriptor.IsControllerAction())
            {
                throw new Exception($"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
            }

            return actionDescriptor as ControllerActionDescriptor;
        }

        public static bool IsObjectResult(this ActionDescriptor actionDescriptor, params Type[] excludeTypes)
        {
            var returnType = actionDescriptor.AsControllerActionDescriptor().MethodInfo.ReturnType;
            if (returnType == typeof(Task))
            {
                returnType = typeof(void);
            }

            if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GenericTypeArguments[0];
            }


            if (!excludeTypes.IsNullOrEmpty() && excludeTypes.Any(t => t.IsAssignableFrom(returnType)))
            {
                return false;
            }

            if (!typeof(IActionResult).IsAssignableFrom(returnType))
            {
                return true;
            }

            return ObjectResultTypes.Any(t => t.IsAssignableFrom(returnType));
        }
    }
}