using System;

namespace Sprite.EventBus
{
    /// <summary>
    /// 事件
    /// </summary>
    public interface IEvent
    {
        public string Id { get; }

        public DateTime CreationTime { get; }
    }
}