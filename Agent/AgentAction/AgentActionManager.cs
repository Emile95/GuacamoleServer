using API.Server;
using API.AgentAction;

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

        public void ProcessAction(string actionId, string runningAgentActionId)
        {
            AgentActionContext agentActionContext = new AgentActionContext();
            agentActionContext.ServerOperations = _serverOperations;
            agentActionContext.RunningAgentActionId = runningAgentActionId;
            _agentActionsLoaded[actionId].Invoke(agentActionContext);
        }

        public bool IsValidActionId(string id)
        {
            return _agentActionsLoaded.ContainsKey(id);
        }
    }
}
