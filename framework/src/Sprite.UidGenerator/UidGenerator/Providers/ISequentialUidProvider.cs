using Sprite.UidGenerator;

namespace Sprite.UidGenerator
{
    /// <summary>
    /// 顺序UID提供程序
    /// </summary>
    /// <typeparam name="T">可表达的基础值类型或者引用类型(int/Guid )</typeparam>
    public interface ISequentialUidProvider<T> 
    {
        /// <summary>
        /// 获取下一个Id
        /// Get Next Id
        /// </summary>
        /// <returns>NextId</returns>
        T NextId();
    }
}