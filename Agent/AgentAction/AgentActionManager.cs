using Agent.ServerApplication;
using API.AgentAction;

namespace AgentAction
{
    public class AgentActionManager
    {
        private readonly ServerClient _serverClient;

        protected readonly Dictionary<string, Action<AgentActionContext>> _agentActionsLoaded;

        public AgentActionManager(ServerClient serverClient)
        {
            _serverClient = serverClient;
            _agentActionsLoaded = new Dictionary<string, Action<AgentActionContext>>();
        }

        public bool AddAgentAction(string id, Action<AgentActionContext> action)
        {
            _agentActionsLoaded.Add(id, action);
            return true;
        }

        public void ProcessAction(string actionId, string agentActionId)
        {
            AgentActionContext agentActionContext = new AgentActionContext();
            _agentActionsLoaded[actionId].Invoke(agentActionContext);
            _serverClient.FinishAction(agentActionId);
        }

        public bool IsValidActionId(string id)
        {
            return _agentActionsLoaded.ContainsKey(id);
        }
    }
}
