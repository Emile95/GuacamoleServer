using Library.Agent.Configuration.EventHandler;

namespace Library.Agent.EventHandler
{
    public class AgentEventHandlerManager
    {
        private readonly Dictionary<AgentEventHandlerType, List<Action<AgentEventHandlerContext>>> _eventHandlers;

        public AgentEventHandlerManager()
        {
            _eventHandlers = new Dictionary<AgentEventHandlerType, List<Action<AgentEventHandlerContext>>>();
            _eventHandlers.Add(AgentEventHandlerType.BeforeHttpRequest, new List<Action<AgentEventHandlerContext>>());
            _eventHandlers.Add(AgentEventHandlerType.AfterHttpRequest, new List<Action<AgentEventHandlerContext>>());
        }

        public void Add(Action<AgentEventHandlerContext> eventHandler, IEnumerable<AgentEventHandlerAttribute> eventHandlerAttributes)
        {
            foreach(AgentEventHandlerAttribute eventHandlerAttribute in eventHandlerAttributes)
                _eventHandlers[eventHandlerAttribute.EventType].Add(eventHandler);
        }

        public void Add(Action<AgentEventHandlerContext> eventHandler, AgentEventHandlerType eventHandlerType)
        {
            _eventHandlers[eventHandlerType].Add(eventHandler);
        }

        public void CallEventHandlers(AgentEventHandlerType eventHandlerType, AgentEventHandlerContext context)
        {
            for (int i = 0; i < _eventHandlers[eventHandlerType].Count; i++)
                _eventHandlers[eventHandlerType][i](context);
        }
    }
}
