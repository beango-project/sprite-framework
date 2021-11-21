using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AspectInjector.Broker;
using FastExpressionCompiler;
using ImTools;

namespace Sprite.Aspects
{
    public abstract class AroundAspect
    {
        private static readonly Type _voidTaskResult = Type.GetType("System.Threading.Tasks.VoidTaskResult");
        private static ImHashMap<MethodBase, Handler> _delegateCache = ImHashMap<MethodBase, Handler>.Empty;

        private delegate object Handler(Func<object[], object> next, object[] args, AspectEventArgs eventArgs);

        private static readonly MethodInfo _handler = typeof(AroundAspect).GetMethod(nameof(AroundAspect.Sync));
        private static readonly MethodInfo _handlerAsync = typeof(AroundAspect).GetMethod(nameof(AroundAspect.Async));

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

            var wrappers = triggers.OfType<AroundAspect>().ToArray();

            var handler = GetMethodHandler(method, returnType, instance);
            return handler(target, args, eventArgs);
        }

        private Handler GetMethodHandler(MethodBase method, Type returnType, object instance)
        {
            if (!_delegateCache.TryFind(method, out var handler))
            {
                if (!_delegateCache.TryFind(method, out handler))
                {
                    handler = CreateMethodHandler(method, returnType, instance);
                    _delegateCache = _delegateCache.AddOrUpdate(method, handler);
                }
            }

            return handler;
        }

        private Handler CreateMethodHandler(MethodBase method, Type returnType, object instance)
        {
            var targetParam = Expression.Parameter(typeof(Func<object[], object>), "orig");
            var eventArgsParam = Expression.Parameter(typeof(AspectEventArgs), "event");

            MethodInfo wrapperMethod;

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                var taskType = returnType.IsConstructedGenericType ? returnType.GenericTypeArguments[0] : _voidTaskResult;
                returnType = typeof(Task<>).MakeGenericType(new[] { taskType });

                wrapperMethod = _handlerAsync.MakeGenericMethod(new[] { taskType });
            }
            else
            {
                if (returnType == typeof(void))
                    returnType = typeof(object);

                wrapperMethod = _handler.MakeGenericMethod(new[] { returnType });
            }

            var converArgs = Expression.Parameter(typeof(object[]), "args");
            var next = Expression.Lambda(Expression.Convert(Expression.Invoke(targetParam, converArgs), returnType), converArgs);

            // foreach (var wrapper in wrappers)
            // {
            //     var argsParam = Expression.Parameter(typeof(object[]), "args");
            //     next = Expression.Lambda(Expression.Call(Expression.Constant(wrapper), wrapperMethod, next, argsParam, eventArgsParam), argsParam);
            // }

            var argsParam = Expression.Parameter(typeof(object[]), "args");
            next = Expression.Lambda(Expression.Call(Expression.Constant(instance), wrapperMethod, next, argsParam, eventArgsParam), argsParam);
            var orig_args = Expression.Parameter(typeof(object[]), "orig_args");
            var handler = Expression.Lambda<Handler>(Expression.Convert(Expression.Invoke(next, orig_args), typeof(object)), targetParam, orig_args, eventArgsParam);

            var handlerCompiled = handler.CompileFast();

            return handlerCompiled;
        }


        public T Sync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
        {
            return Handle(target, args, eventArgs);
            // return target(args);
        }

        public Task<T> Async<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
        {
            return HandleAsync(target, args, eventArgs);
            // return target(args);
        }

        protected virtual T Handle<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
        {
            return target(args);
        }

        protected virtual Task<T> HandleAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
        {
            return target(args);
        }
    }
}