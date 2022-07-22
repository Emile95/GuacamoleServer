using Library.Agent.EventHandler;

namespace Library.Agent.Configuration.EventHandler
{
    public class AgentEventHandlerAttribute : Attribute
    {
        public AgentEventHandlerType EventType { get; set; }
    }
}
