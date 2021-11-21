using System.Threading.Tasks;

namespace Sprite.EventBus
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    public interface IEventHandler<in TEventData> : IEventHandler
    {
        Task HandleAsync(TEventData eventData);
    }

    public interface IEventHandler
    {
    }
}