using FastExpressionCompiler.LightExpression;

namespace System.Reflection
{
    /// <summary>
    /// 用于创建大量的对象
    /// 使用快速表达式树编译光表达式来创建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class New<T>
        where T : new()
    {
        public static Func<T> Instance = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).CompileFast();
    }
}