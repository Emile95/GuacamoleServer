

using API.Server.EventHandler;

namespace API.Server.Configuration.EventHandler
{
    public class ServerEventHandlerAttribute : Attribute
    {
        public ServerEventHandlerType EventType { get; set; }
    }
}
