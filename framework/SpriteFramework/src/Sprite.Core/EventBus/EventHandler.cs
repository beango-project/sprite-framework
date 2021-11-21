using System;
using System.Threading.Tasks;

namespace Sprite.EventBus
{
    public class EventHandler<TEventData> : IEventHandler<TEventData>
    {
        public Func<TEventData, Task> Action { get; private set; }


        public EventHandler(Func<TEventData, Task> action)
        {
            Action = action;
        }

        public async Task HandleAsync(TEventData eventData)
        {
            await Action(eventData);
        }
    }
}