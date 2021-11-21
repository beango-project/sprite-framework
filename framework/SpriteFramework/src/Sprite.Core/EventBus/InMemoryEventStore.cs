using System;
using System.Collections.Generic;
using System.Linq;
using ImTools;

namespace Sprite.EventBus
{
    public class InMemoryEventStore : IEventStore
    {
        private ImHashMap<string, HashSet<Type>> _handlers = ImHashMapEntry<string, HashSet<Type>>.Empty;

        private readonly List<Type> _eventTypes;

        public InMemoryEventStore()
        {
            _eventTypes = new List<Type>();
        }

        public void AddSubscription<TEvent, TEventHandler>() where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            AddSubscriptionHandler(typeof(TEventHandler), eventKey);

            _handlers.GetValueOrDefault(eventKey).Add(typeof(TEventHandler));
            _eventTypes.Add(typeof(TEvent));
        }

        public void Clear()
        {
            _handlers = ImHashMapEntry<string, HashSet<Type>>.Empty;
        }

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).Name;
        }

        public void GetEventKey<TEvent>(out string eventKey)
        {
            eventKey = typeof(TEvent).Name;
        }

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.Contains(eventName);

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : IEvent
        {
            var key = GetEventKey<TEvent>();
            return HasSubscriptionsForEvent(key);
        }

        public void RemoveSubscription<TEvent, TEventHandler>() where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            if (HasSubscriptionsForEvent(eventKey))
            {
                _handlers.GetValueOrDefault(eventKey).Remove(typeof(TEventHandler));
            }
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : IEvent
        {
            if (_handlers.IsEmpty)
            {
                return Array.Empty<Type>();
            }

            var eventKey = GetEventKey<TEvent>();
            if (_handlers.TryFind(eventKey, out var handlers))
            {
                return handlers;
            }

            return Array.Empty<Type>();
        }


        /// <summary>
        /// 拦截处理添加的订阅，添加拦截器
        /// </summary>
        /// <param name="handlerType">处理器类型</param>
        /// <param name="eventName">事件名称</param>
        private void AddSubscriptionHandler(Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers = _handlers.AddOrUpdate(eventName, new HashSet<Type>());
            }

            if (_handlers.GetValueOrDefault(eventName).Any(t => t == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }
        }
    }
}