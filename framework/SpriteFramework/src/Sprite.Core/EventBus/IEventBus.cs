using System.Threading.Tasks;

namespace Sprite.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;

        void Subscribe<TEvent, TEventHandler>() where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>;

        void Unsubscribe();
    }
}