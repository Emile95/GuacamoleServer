

using Library.Server.EventHandler;

namespace Library.Server.Configuration.EventHandler
{
    public class ServerEventHandlerAttribute : Attribute
    {
        public ServerEventHandlerType EventType { get; set; }
    }
}
