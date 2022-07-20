using Library.Agent.Action;

namespace Library.Action
{
    public abstract class AgentActionManager<InstanceType>
    {
        protected readonly Dictionary<string, AgentActionLoaded<InstanceType>> _agentActionsLoaded;

        public AgentActionManager()
        {
            _agentActionsLoaded = new Dictionary<string, AgentActionLoaded<InstanceType>>();
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
