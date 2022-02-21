using Library.Configuration.Module.EventHandler;

namespace Library.EventHandler
{
    public class EventHandlerManager
    {
        private readonly Dictionary<EventHandlerType, List<Action<EventHandlerContext>>> _eventHandlers;

        public EventHandlerManager()
        {
            _eventHandlers = new Dictionary<EventHandlerType, List<Action<EventHandlerContext>>>();
            _eventHandlers.Add(EventHandlerType.AfterActionRun, new List<Action<EventHandlerContext>>());
            _eventHandlers.Add(EventHandlerType.BeforeActionRun, new List<Action<EventHandlerContext>>());
        }

        public void AddEventHandler(Action<EventHandlerContext> eventHandler, IEnumerable<EventHandlerAttribute> eventHandlerAttributes)
        {
            foreach(EventHandlerAttribute eventHandlerAttribute in eventHandlerAttributes)
                _eventHandlers[eventHandlerAttribute.EventHandlerType].Add(eventHandler);
        }

        public void CallEventHandlers(EventHandlerType eventHandlerType, EventHandlerContext context)
        {
            for (int i = 0; i < _eventHandlers[eventHandlerType].Count; i++)
                _eventHandlers[eventHandlerType][i](context);
        }
    }
}
