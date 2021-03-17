namespace Sprite.ObjectMapping
{
    /// <summary>
    /// 对象映射器接口
    /// </summary>
    public interface IObjectMapper
    {
        /// <summary>
        /// 将源对象转换（映射）成类型为 TDestination 的一个新建对象
        /// </summary>
        /// <typeparam name="TDestination">转换目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        TDestination Map<TDestination>(object source);

        /// <summary>
        /// 将源对象转换（映射）成类型为 TDestination 的一个已经存在的对象
        /// </summary>
        /// <typeparam name="TSource">源对象类型</typeparam>
        /// <typeparam name="TDestination">转换目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <returns>映射处理后的目标对象</returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}