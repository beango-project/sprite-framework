namespace Sprite.UidGenerator.Providers
{
    /// <summary>
    /// 雪花算法Uid提供程序
    /// </summary>
    public interface ISnowflakeUidProvider : ISequentialUidProvider<long>, IUidProvider<long>
    {
    }
}