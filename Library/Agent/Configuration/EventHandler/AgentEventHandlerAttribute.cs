using API.Agent.EventHandler;

namespace API.Agent.Configuration.EventHandler
{
    public class AgentEventHandlerAttribute : Attribute
    {
        public AgentEventHandlerType EventType { get; set; }
    }
}
