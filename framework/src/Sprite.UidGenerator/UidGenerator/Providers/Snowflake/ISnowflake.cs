using System;

namespace Sprite.UidGenerator.Providers.Snowflake
{
    /// <summary>
    /// 雪花算法
    /// </summary>
    internal interface ISnowflake
    {
        Action<OverCostActionArg> GenAction { get; set; }
        
        long NextId();
    }
}