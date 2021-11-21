namespace Sprite.UidGenerator
{
    /// <summary>
    /// 全局ID提供者
    /// </summary>
    public interface IUidProvider<out T>
    {
        T Create();
    }
}