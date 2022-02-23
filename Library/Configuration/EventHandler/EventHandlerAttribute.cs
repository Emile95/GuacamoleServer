using Library.EventHandler;

namespace Library.Configuration.EventHandler
{
    public class EventHandlerAttribute : Attribute
    {
        public EventHandlerType EventHandlerType { get; private set; }

        public EventHandlerAttribute(EventHandlerType eventHandlerType)
        {
            EventHandlerType = eventHandlerType;
        }
    }
}
