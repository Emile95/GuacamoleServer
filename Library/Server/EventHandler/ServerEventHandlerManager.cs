using Library.Server.Configuration.EventHandler;

namespace Library.Server.EventHandler
{
    public class ServerEventHandlerManager
    {
        private readonly Dictionary<ServerEventHandlerType, List<Action<ServerEventHandlerContext>>> _eventHandlers;

        public ServerEventHandlerManager()
        {
            _eventHandlers = new Dictionary<ServerEventHandlerType, List<Action<ServerEventHandlerContext>>>();
            _eventHandlers.Add(ServerEventHandlerType.BeforeHttpRequest, new List<Action<ServerEventHandlerContext>>());
            _eventHandlers.Add(ServerEventHandlerType.AfterHttpRequest, new List<Action<ServerEventHandlerContext>>());
        }

        public void Add(Action<ServerEventHandlerContext> eventHandler, IEnumerable<ServerEventHandlerAttribute> eventHandlerAttributes)
        {
            foreach(ServerEventHandlerAttribute eventHandlerAttribute in eventHandlerAttributes)
                _eventHandlers[eventHandlerAttribute.EventType].Add(eventHandler);
        }

        public void Add(Action<ServerEventHandlerContext> eventHandler, ServerEventHandlerType eventHandlerType)
        {
            _eventHandlers[eventHandlerType].Add(eventHandler);
        }

        public void CallEventHandlers(ServerEventHandlerType eventHandlerType, ServerEventHandlerContext context)
        {
            for (int i = 0; i < _eventHandlers[eventHandlerType].Count; i++)
                _eventHandlers[eventHandlerType][i](context);
        }
    }
}
