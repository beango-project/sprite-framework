using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Sprite.Remote;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    /// <summary>
    /// 规约控制器描述
    /// </summary>
    public class ConventionControllerDescription
    {
        private string _remoteServiceName;

        private string _rootPath;

        public ConventionControllerDescription(Assembly fromAssembly, string rootPath, string remoteServiceName)
        {
            FromAssembly = fromAssembly;
            RootPath = rootPath;
            RemoteServiceName = remoteServiceName;
            ApiVersions = new List<ApiVersion>();
            ControllerTypes = new HashSet<Type>();
        }

        /// <summary>
        /// 从程序集
        /// </summary>
        public Assembly FromAssembly { get; }

        /// <summary>
        /// 控制器类型
        /// </summary>
        public HashSet<Type> ControllerTypes { get; }

        [CanBeNull]
        public Func<ControllerRouterGuide, string> ControllerRouteMap { get; set; }

        [CanBeNull]
        public Func<ActionRouteGuide, string> ActionRouteMap { get; set; }

        public Action<ApiVersioningOptions> ApiVersionOptions { get; set; }
        public Action<ControllerModel> ControllerOptions { get; }

        /// <summary>
        /// Api版本
        /// </summary>
        public List<ApiVersion> ApiVersions { get; }

        /// <summary>
        /// 类型过滤器
        /// </summary>
        public Func<Type, bool> Filter { get; set; }

        /// <summary>
        /// 是否使用RESTful风格的Action
        /// </summary>
        public bool? UseRestfulActionName { get; set; }

        [NotNull]
        public string RootPath
        {
            get => _rootPath;
            set
            {
                Check.NotNull(value, nameof(value));
                _rootPath = value;
            }
        }

        [NotNull]
        public string RemoteServiceName
        {
            get => _remoteServiceName;
            set
            {
                Check.NotNull(value, nameof(value));
                _remoteServiceName = value;
            }
        }

        public void Load()
        {
            var types = FromAssembly.GetTypes().Where(IsCandidate);
            if (Filter != null)
            {
                types = types.Where(Filter);
            }

            foreach (var type in types)
            {
                ControllerTypes.Add(type);
            }
        }

        /// <summary>
        /// 符合候选，继承<see cref="IRemoteService" />接口，或标记使用了<see cref="RemoteAttribute" />的类
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        protected virtual bool IsCandidate(Type type)
        {
            if (!type.IsPublic || type.IsAbstract || type.IsGenericType)
            {
                return false;
            }

            var remoteServiceAttr = type.GetAttributeWithDefined<RemoteServiceAttribute>();
            if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabled)
            {
                return false;
            }

            if (typeof(IRemoteService).IsAssignableFrom(type))
            {
                return true;
            }

            return false;
        }
    }
}