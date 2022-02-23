using Library.EventHandler;

namespace Library.Configuration.EventHandler
{
    public class EventHandlerAttribute : Attribute
    {
        public EventHandlerType EventType { get; set; }
    }
}
