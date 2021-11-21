namespace Sprite.Data.Transaction
{
    /// <summary>
    /// 事务传播级别
    /// </summary>
    public enum Propagation
    {
        /// <summary>
        /// 表示当前方法必须在一个具有事务的上下文中运行,如有客户端有事务在进行，那么被调用端将在该事务中运行，否则的话重新开启一个事务。( 如果被调用端发生异常,那
        /// 么调用端和被调用端事务都将回滚)
        /// </summary>
        Required,

        /// <summary>
        /// 表示当前方法不必需要具有一个事务上下文,但是如果有一个事务的话,它也可以在这个事务中运行
        /// </summary>
        Supports,

        /// <summary>
        /// 表示当前方法必须在一个事务中运行，如果没有事务，将抛出异常
        /// </summary>
        Mandatory,

        /// <summary>
        /// 总是开启一个新的事务。如果一个事务已经存在，则将这个存在的事务挂起。
        /// </summary>
        RequiresNew,

        /// <summary>
        /// 总是非事务地执行，并挂起任何存在的事务。
        /// </summary>
        NotSupported,

        /// <summary>
        /// 总是非事务地执行，如果存在一个活动事务，则抛出异常
        /// </summary>
        Never,

        /// <summary>
        /// 事务嵌套
        /// </summary>
        Nested,
        
        Auto,
    }
}