using API.AgentAction;

namespace API.AgentAction
{
    public class RunningAgentActionLog
    {
        public string RunningAgentActionId { get; set; }
        public RunningAgentActionLogType RunningAgentActionLogType { get; set; }
        public string Message { get; set; }
    }
}
