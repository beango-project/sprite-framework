using System;
using System.Collections.Generic;

namespace Sprite.EventBus
{
    public interface IEventStore
    {
        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void AddSubscription<TEvent, TEventHandler>() where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>;

        void Clear();

        string GetEventKey<TEvent>();

        void GetEventKey<TEvent>(out string eventKey);

        bool HasSubscriptionsForEvent(string eventName);

        bool HasSubscriptionsForEvent<TEvent>() where TEvent : IEvent;

        void RemoveSubscription<TEvent, TEventHandler>() where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>;

        ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : IEvent;
    }
}