using Library.Agent.Action;

namespace Library.Action
{
    public abstract class AgentActionManager<InstanceType>
    {
        protected readonly Dictionary<string, AgentActionLoaded<InstanceType>> _agentActionsLoaded;
        protected readonly Dictionary<string, RunningAgentActionLogs> _runningAgentActions;

        public AgentActionManager()
        {
            _agentActionsLoaded = new Dictionary<string, AgentActionLoaded<InstanceType>>();
            _runningAgentActions = new Dictionary<string, RunningAgentActionLogs>();
        }

        public bool IsActionIdValid(string actionId)
        {
            return _agentActionsLoaded.ContainsKey(actionId);
        }

        public List<AgentActionLoaded<InstanceType>> GetLoadedAgentActions()
        {
            return _agentActionsLoaded.Values.ToList();
        }

        public abstract bool AddAgentAction(AgentActionLoaded<InstanceType> agentActionLoaded);
    }
}
