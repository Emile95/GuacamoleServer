namespace API.AgentAction
{
    public class AgentActionContext
    {
        public Action<string> FinishAgentAction { get; set; }
        public Action<string> FatalAgentAction { get; set; }
        public Action<string> LogInfoAgentAction { get; set; }
        public Action<string> LogWarningAgentAction { get; set; }
        public Action<string> LogErrorAgentAction { get; set; }
    }
}
