using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectInjector.Broker;
using FastExpressionCompiler.LightExpression;
using ImTools;
using Sprite.Data.Transaction;
using Sprite.DependencyInjection;

namespace Sprite.Data.Uow
{
    [Aspect(Scope.Global, Factory = typeof(ApplicationServices))]
    public class UnitOfWorkAspect
    {
        private static readonly MethodInfo _handler = typeof(UnitOfWorkAspect).GetMethod(nameof(UnitOfWorkAspect.Handle));
        private static readonly MethodInfo _handlerAsync = typeof(UnitOfWorkAspect).GetMethod(nameof(UnitOfWorkAspect.HandleAsync));
        private static readonly Type _voidTaskResult = Type.GetType("System.Threading.Tasks.VoidTaskResult");

        private static ImHashMap<MethodBase, Handler> _delegateCache = ImHashMap<MethodBase, Handler>.Empty;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private delegate object Handler(Func<object[], object> target, object[] args, IUnitOfWork unitOfWork);

        public UnitOfWorkAspect(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [Advice(Kind.Around, Targets = Target.Method)]
        public object HandleBind(
            [Argument(Source.Instance)] object instance,
            [Argument(Source.Type)] Type type,
            [Argument(Source.Metadata)] MethodBase method,
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Name)] string name,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.ReturnType)] Type returnType,
            [Argument(Source.Triggers)] Attribute[] triggers)
        {
            var transactional = triggers.Cast<TransactionalAttribute>().SingleOrDefault();
            TransactionOptions options = new TransactionOptions()
            {
                Propagation = transactional.Propagation,
                IsolationLevel = transactional.IsolationLevel,
                Timeout = transactional.Timeout
            };

            IUnitOfWork unitOfWork = null;
            if (_unitOfWorkManager.TryBeginReserved("ReservedUow", options))
            {
                unitOfWork = _unitOfWorkManager.CurrentUow;
            }
            else
            {
                unitOfWork = _unitOfWorkManager.Begin(options);
            }
            // var unitOfWork = _unitOfWorkManager.Begin(options);

            var methodHandler = GetMethodHandler(method, returnType);
            return methodHandler(target, args, unitOfWork);
        }


        private Handler GetMethodHandler(MethodBase method, Type returnType)
        {
            if (!_delegateCache.TryFind(method, out var handler))
            {
                if (!_delegateCache.TryFind(method, out handler))
                {
                    handler = CreateMethodHandler(method, returnType);
                    _delegateCache = _delegateCache.AddOrUpdate(method, handler);
                }
            }

            return handler;
        }

        private Handler CreateMethodHandler(MethodBase method, Type returnType)
        {
            var targetParam = Expression.Parameter(typeof(Func<object[], object>), "orig");

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
            // var optionsParam = Expression.Parameter(typeof(object[]), "options");

            var uowArgs = Expression.Parameter(typeof(IUnitOfWork), "unitOfWork");

            next = Expression.Lambda(Expression.Call(null, wrapperMethod, next, argsParam, uowArgs), argsParam, uowArgs);
            var orig_args = Expression.Parameter(typeof(object[]), "orig_args");


            var handler = Expression.Lambda<Handler>(Expression.Convert(Expression.Invoke(next, orig_args), typeof(object)), targetParam, orig_args, uowArgs);

            var handlerCompiled = handler.CompileFast();

            return handlerCompiled;
        }

        #region Obsolete code

        //
        // [Advice(Kind.Around, Targets = Target.Method)]
        // public object Handle(
        //     [Argument(Source.Instance)] object instance,
        //     [Argument(Source.Type)] Type type,
        //     [Argument(Source.Metadata)] MethodBase method,
        //     [Argument(Source.Target)] Func<object[], object> target,
        //     [Argument(Source.Name)] string name,
        //     [Argument(Source.Arguments)] object[] args,
        //     [Argument(Source.ReturnType)] Type returnType,
        //     [Argument(Source.Triggers)] Attribute[] triggers)
        // {
        //     return base.Handle(instance, type, method, target, name, args, returnType, triggers);
        // }

        //
        // [Advice(Kind.Around)]
        // public object Handle(
        //     [Argument(Source.Target)] Func<object[], object> target,
        //     [Argument(Source.Arguments)] object[] args,
        //     [Argument(Source.Name)] string name,
        //     [Argument(Source.Metadata)] MethodBase method,
        //     [Argument(Source.ReturnType)] Type retType,
        //     [Argument(Source.Triggers)] Attribute[] triggers
        // )
        // {
        //     var transactional = triggers.Cast<TransactionalAttribute>().FirstOrDefault();
        //     TransactionOptions options = new TransactionOptions()
        //     {
        //         TransactionPropagation = transactional.Propagation,
        //         IsolationLevel = transactional.IsolationLevel,
        //         Timeout = transactional.Timeout
        //     };
        //
        //     _unitOfWorkManager.Begin(options);
        //
        //     if (typeof(Task).IsAssignableFrom(retType)) //check if method is async, you can also check by statemachine attribute
        //     {
        //         var syncResultType = retType.IsConstructedGenericType ? retType.GenericTypeArguments[0] : _voidTaskResult;
        //         var tgt = target;
        //         //if (!retType.IsConstructedGenericType)
        //         //    tgt = new Func<object[], object>(a=>((Task)target(a)).ContinueWith(t=> (object)null));
        //         return _asyncHandler.MakeGenericMethod(syncResultType).Invoke(this, new object[] { tgt, args, name });
        //     }
        //     else
        //     {
        //         retType = retType == typeof(void) ? typeof(object) : retType;
        //         return _syncHandler.MakeGenericMethod(retType).Invoke(this, new object[] { target, args, name });
        //     }
        // }
        //
        //
        // private static T WrapSync<T>(Func<object[], object> target, object[] args, string name)
        // {
        //     try
        //     {
        //         var result = (T)target(args);
        //         Console.WriteLine($"Sync method `{name}` completes successfuly.");
        //         return result;
        //     }
        //     catch (TargetInvocationException e)
        //     {
        //         Console.WriteLine($"Sync method `{name}` throws {e.GetType()} exception.");
        //         return default;
        //     }
        // }
        //
        // private static async Task<T> WrapAsync<T>(Func<object[], object> target, object[] args, string name)
        // {
        //     T result = default(T);
        //     try
        //     {
        //         result = await ((Task<T>)target(args)).ConfigureAwait(false);
        //         // Console.WriteLine($"Async method `{name}` completes successfuly.");
        //         await AmbientUnitOfWork.Current.UnitOfWork.CompletedAsync();
        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         // Console.WriteLine($"Async method `{name}` throws {ex.GetType()} exception.");
        //         await AmbientUnitOfWork.Current.UnitOfWork?.RollbackAsync();
        //         throw ex;
        //     }
        //     finally
        //     {
        //         AmbientUnitOfWork.Current.UnitOfWork?.Dispose();
        //     }
        //
        // }

        // [Advice(Kind.Before)]
        // public void Handle([Argument(Source.Metadata)] MethodBase method)
        // {
        //     var transactional = method.GetCustomAttribute<TransactionalAttribute>();
        //
        //     TransactionOptions options = new TransactionOptions()
        //     {
        //         TransactionPropagation = transactional.Propagation,
        //         IsolationLevel = transactional.IsolationLevel,
        //         Timeout = transactional.Timeout
        //     };
        //     var unitOfWork = _unitOfWorkManager.Begin(options);
        //     // Console.WriteLine($"当前事务传的播配置为:{transactional.Propagation.ToString()}");
        //     // Console.WriteLine($"开启工作单元:{unitOfWork}【{unitOfWork.Id}】");
        //     var current = _unitOfWorkManager.Current;
        //     // Console.WriteLine($"当前工作单元:{current}【{current.Id}】");
        // }
        //
        // [Advice(Kind.After)]
        // public void HandleAfter()
        // {
        //     var current = _unitOfWorkManager.Current;
        //     current.Completed();
        //     // Console.WriteLine($"销毁工作单元:{current}【{current.Id}】");
        //     _unitOfWorkManager.Current?.Dispose();
        //     // Console.WriteLine($"销毁后当前工作单元:{_unitOfWorkManager.Current?.ToString() ?? "无"}【{_unitOfWorkManager.Current?.Id ?? Guid.Empty}】");
        // }
        // protected override T HandleSync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // protected override async Task<T> HandleAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
        // {
        //     var transactional = eventArgs.Triggers.Cast<TransactionalAttribute>().FirstOrDefault();
        //     TransactionOptions options = new TransactionOptions()
        //     {
        //         TransactionPropagation = transactional.Propagation,
        //         IsolationLevel = transactional.IsolationLevel,
        //         Timeout = transactional.Timeout
        //     };
        //     T res = default;
        //     try
        //     {
        //         _unitOfWorkManager.Begin(options);
        //         res = await target(args);
        //         await _unitOfWorkManager.CurrentUow.CompletedAsync();
        //     }
        //     catch (Exception exception)
        //     {
        //         await _unitOfWorkManager.CurrentUow.RollbackAsync();
        //         throw exception;
        //     }
        //     finally
        //     {
        //         _unitOfWorkManager.CurrentUow.Dispose();
        //     }
        //
        //     return res;
        // }

        #endregion


        public static T Handle<T>(Func<object[], T> target, object[] args, IUnitOfWork unitOfWork)
        {
            try
            {
                var result = target(args);
                // AmbientUnitOfWork.Current.UnitOfWork?.Completed();
                if (!unitOfWork.IsCompleted)
                {
                    unitOfWork.Completed();
                }

                return result;
            }
            catch (TargetInvocationException exception)
            {
                // AmbientUnitOfWork.Current.UnitOfWork?.Rollback();
                unitOfWork.Rollback();
                throw exception;
            }
            finally
            {
                // AmbientUnitOfWork.Current.UnitOfWork?.Dispose();
                unitOfWork.Dispose();
            }
        }

        public static async Task<T> HandleAsync<T>(Func<object[], Task<T>> target, object[] args, IUnitOfWork unitOfWork)
        {
            // T result = default(T);
            try
            {
                var result = await target(args).ConfigureAwait(false);
                // Console.WriteLine($"Async method `{name}` completes successfuly.");
                // await AmbientUnitOfWork.Current.UnitOfWork?.CompletedAsync();
                if (!unitOfWork.IsCompleted)
                {
                    await unitOfWork.CompletedAsync();
                }

                return result;
            }
            catch (TargetInvocationException exception)
            {
                // Console.WriteLine($"Async method `{name}` throws {ex.GetType()} exception.");
                // await AmbientUnitOfWork.Current.UnitOfWork?.RollbackAsync();
                await unitOfWork.RollbackAsync();
                throw exception;
            }
            finally
            {
                // AmbientUnitOfWork.Current.UnitOfWork?.Dispose();
                if (unitOfWork is UnitOfWork && unitOfWork.ReservationKey == "ReservedUow")
                {
                }
                else
                {
                    await unitOfWork.DisposeAsync();
                }
            }
        }
    }
}