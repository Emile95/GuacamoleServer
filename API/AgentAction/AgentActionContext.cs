using API.Server;

namespace API.AgentAction
{
    public class AgentActionContext
    {
        public string RunningAgentActionId { get; set; }
        public ServerOperations ServerOperations { get; set;}
    }
}
