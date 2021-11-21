using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectInjector.Broker;
using FastExpressionCompiler.LightExpression;
using ImTools;

namespace Sprite.Aspects
{
    [Aspect(Scope.Global, Factory = typeof(ApplicationServices))]
    public class MethodAspect : AspectBase
    {
        [Advice(Kind.Around, Targets = Target.Method)]
        public object Handle(
            [Argument(Source.Instance)] object instance,
            [Argument(Source.Type)] Type type,
            [Argument(Source.Metadata)] MethodBase method,
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Name)] string name,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.ReturnType)] Type returnType,
            [Argument(Source.Triggers)] Attribute[] triggers)
        {
            return base.Handle(instance, type, method, target, name, args, returnType, triggers);
        }

        protected override T HandleSync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        protected override Task<T> HandleAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        // protected internal virtual T HandleSync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
        // {
        //     return target(args);
        // }
        //
        // protected internal virtual Task<T> HandleAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
        // {
        //     return target(args);
        // }
    }


    public abstract class AspectBase
    {
        private static ImHashMap<MethodBase, Handler> _delegateCache = ImHashMap<MethodBase, Handler>.Empty;

        private delegate object Method(object[] args);

        private delegate object Wrapper(Func<object[], object> target, object[] args);

        private delegate object Handler(Func<object[], object> next, object[] args, AspectEventArgs eventArgs);

        private static readonly MethodInfo _asyncGenericHandler =
            typeof(AspectBase).GetMethod(nameof(HandleAsync), BindingFlags.NonPublic | BindingFlags.Instance);


        private static readonly MethodInfo _syncGenericHandler =
            typeof(AspectBase).GetMethod(nameof(HandleSync), BindingFlags.NonPublic | BindingFlags.Instance);

        protected virtual object Handle(object instance, Type type, MethodBase method, Func<object[], object> target, string name, object[] args, Type returnType,
            Attribute[] triggers)
        {
            var eventArgs = new AspectEventArgs
            {
                Instance = instance,
                Type = type,
                Method = method,
                Name = name,
                Args = args,
                ReturnType = returnType,
                Triggers = triggers
            };

            var wrappers = triggers.OfType<MethodAspectAttribute>().ToArray();

            var handler = GetMethodHandler(method, returnType, wrappers);
            return handler(target, args, eventArgs);
        }


        private Handler GetMethodHandler(MethodBase method, Type returnType, IReadOnlyList<MethodAspectAttribute> wrappers)
        {
            if (!_delegateCache.TryFind(method, out var handler))
            {
                if (!_delegateCache.TryFind(method, out handler))
                {
                    handler = CreateMethodHandler(method, returnType, wrappers);
                    _delegateCache = _delegateCache.AddOrUpdate(method, handler);
                }
            }

            return handler;
        }

        private Handler CreateMethodHandler(MethodBase method, Type returnType, IReadOnlyList<MethodAspectAttribute> wrappers)
        {
            var targetParam = Expression.Parameter(typeof(Func<object[], object>), "orig");
            var eventArgsParam = Expression.Parameter(typeof(AspectEventArgs), "event");

            MethodInfo wrapperMethod;

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                var taskType = returnType.IsConstructedGenericType ? returnType.GenericTypeArguments[0] : Type.GetType("System.Threading.Tasks.VoidTaskResult");
                returnType = typeof(Task<>).MakeGenericType(taskType);

                wrapperMethod = _asyncGenericHandler.MakeGenericMethod(taskType);
            }
            else
            {
                if (returnType == typeof(void))
                    returnType = typeof(object);

                wrapperMethod = _syncGenericHandler.MakeGenericMethod(returnType);
            }

            var converArgs = Expression.Parameter(typeof(object[]), "args");
            var next = Expression.Lambda(Expression.Convert(Expression.Invoke(targetParam, converArgs), returnType), converArgs);

            foreach (var wrapper in wrappers)
            {
                var argsParam = Expression.Parameter(typeof(object[]), "args");
                next = Expression.Lambda(Expression.Call(Expression.Constant(this), wrapperMethod, next, argsParam, eventArgsParam), argsParam);
            }

            var orig_args = Expression.Parameter(typeof(object[]), "orig_args");
            var handler = Expression.Lambda<Handler>(Expression.Convert(Expression.Invoke(next, orig_args), typeof(object)), targetParam, orig_args, eventArgsParam);

            var handlerCompiled = handler.CompileFast();

            return handlerCompiled;
        }

        protected virtual T HandleSync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
        {
            return target.Invoke(args);
        }


        protected virtual async Task<T> HandleAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
        {
            return await target.Invoke(args);
        }
    }
}