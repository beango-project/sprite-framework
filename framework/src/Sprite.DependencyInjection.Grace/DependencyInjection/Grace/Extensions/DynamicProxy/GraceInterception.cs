using System.Reflection;
using Castle.DynamicProxy;
using Sprite.Castle.DynamicProxy;
using Sprite.DynamicProxy;

namespace Grace.DependencyInjection.Extensions.DynamicProxy;

public static class GraceInterception
{
    public static IFluentDecoratorStrategyConfiguration Intercept<TService, TInterceptor>(this IExportRegistrationBlock block, ProxyGenerationOptions options = null, object serviceKey = null)
        where TInterceptor : IInterceptor
    {
        Type decoratorType;

        var tService = typeof(TService);

        if (tService.GetTypeInfo().IsInterface)
        {
            decoratorType = ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(tService, new Type[0],
                ProxyGenerationOptions.Default);
        }
        else if (tService.GetTypeInfo().IsClass)
        {
            decoratorType = ProxyBuilder.CreateClassProxyTypeWithTarget(tService, new Type[0],
                ProxyGenerationOptions.Default);
        }
        else
        {
            throw new Exception($"Service type must be interface or class");
        }

        return serviceKey == null
            ? block.ExportDecorator(decoratorType).As(tService).WithCtorParam<TInterceptor, IInterceptor[]>(i => new IInterceptor[] { i })
            : block.ExportDecorator(decoratorType).As(tService).WithCtorParam<TInterceptor, IInterceptor[]>(i => new IInterceptor[] { i }).LocateWithKey(serviceKey);
    }

    public static IFluentDecoratorStrategyConfiguration Intercept<TService, TInterceptor>(this IExportRegistrationBlock block, TService service, TInterceptor interceptor,
        ProxyGenerationOptions options = null, object serviceKey = null)
        where TInterceptor : IInterceptor
    {
        Type decoratorType;

        var tService = typeof(TService);

        if (tService.GetTypeInfo().IsInterface)
        {
            decoratorType = ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(tService, new Type[0],
                ProxyGenerationOptions.Default);
        }
        else if (tService.GetTypeInfo().IsClass)
        {
            decoratorType = ProxyBuilder.CreateClassProxyTypeWithTarget(tService, new Type[0],
                ProxyGenerationOptions.Default);
        }
        else
        {
            throw new Exception($"Service type must be interface or class");
        }

        return serviceKey == null
            ? block.ExportDecorator(decoratorType).As(tService).WithCtorParam<TInterceptor, IInterceptor[]>(i => new IInterceptor[] { i })
            : block.ExportDecorator(decoratorType).As(tService).WithCtorParam<TInterceptor, IInterceptor[]>(i => new IInterceptor[] { i }).LocateWithKey(serviceKey);
    }

    public static void InterceptAsync<TService, TInterceptor>(this IExportRegistrationBlock block, ProxyGenerationOptions options = null, object serviceKey = null)
        where TInterceptor : class, IAspectInterceptor
    {
        var makeGenericType = typeof(CastleAsyncDeterminationInterceptor<>).MakeGenericType(typeof(TInterceptor));
        block.Export(makeGenericType);
        block.Intercept<TService, CastleAsyncDeterminationInterceptor<TInterceptor>>(options, serviceKey);
    }

    private static DefaultProxyBuilder ProxyBuilder => _proxyBuilder ?? (_proxyBuilder = new DefaultProxyBuilder());

    private static DefaultProxyBuilder _proxyBuilder;

    public static MethodInfo InterceptAsyncMethod = typeof(GraceInterception).GetMethod(nameof(InterceptAsync));
}