namespace Sprite.Caching
{
    /// <summary>
    /// 缓存序列化器
    /// </summary>
    public interface ICacheSerializer
    {
        byte[] Serialize<T>(T obj);

        T Deserialize<T>(byte[] bytes);
    }
}