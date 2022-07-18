using Library.Agent.Action;

namespace Application.Agent.Action
{
    public class AgentActionLoaded
    {
        public string DisplayName { get; set; }
        public string ActionId { get; set; }
        public AgentAction AgentAction { get; set; }
    }
}
