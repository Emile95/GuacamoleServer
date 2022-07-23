using API.AgentAction;
using Agent.ServerApplication;

namespace Agent.AgentAction
{
    public class AgentActionManager
    {
        private readonly ServerOperations _serverOperations;

        protected readonly Dictionary<string, Action<AgentActionContext>> _agentActionsLoaded;

        public AgentActionManager(ServerOperations serverOperations)
        {
            _serverOperations = serverOperations;
            _agentActionsLoaded = new Dictionary<string, Action<AgentActionContext>>();
        }

        public bool AddAgentAction(string id, Action<AgentActionContext> action)
        {
            _agentActionsLoaded.Add(id, action);
            return true;
        }

        public void ProcessAction(string actionId, Tuple<string, object> runningAgentActionData)
        {
            AgentActionContext agentActionContext = new AgentActionContext();
            agentActionContext.FinishAgentAction = (message) => _serverOperations.FinishRunningAgentAction(runningAgentActionData.Item1, message);
            agentActionContext.LogInfoAgentAction = (message) => _serverOperations.LogInfoRunningAgentAction(runningAgentActionData.Item1, message);
            agentActionContext.LogWarningAgentAction = (message) => _serverOperations.LogWarningRunningAgentAction(runningAgentActionData.Item1, message);
            agentActionContext.LogErrorAgentAction = (message) => _serverOperations.LogErrorRunningAgentAction(runningAgentActionData.Item1, message);
            agentActionContext.FatalAgentAction = (message) => _serverOperations.FatalRunningAgentAction(runningAgentActionData.Item1, message);
            agentActionContext.Parameter = runningAgentActionData.Item2;
            _agentActionsLoaded[actionId].Invoke(agentActionContext);
        }

        public bool IsValidActionId(string id)
        {
            return _agentActionsLoaded.ContainsKey(id);
        }
    }
}
