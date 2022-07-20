using Library.Agent.Action;

namespace GuacamoleAgent.Action
{
    public class AgentActionManager
    {
        private readonly Dictionary<string, AgentAction> _agentActions;

        public AgentActionManager()
        {
            _agentActions = new Dictionary<string, AgentAction>();
        }

        public void AddAgentAction(AgentActionLoaded agentActionLoaded)
        {
            AgentAction agentAction = Activator.CreateInstance(agentActionLoaded.Type) as AgentAction;
            _agentActions.Add(agentActionLoaded.ActionId, agentAction);
            Console.WriteLine("New action " + agentActionLoaded.DisplayName + " added");
        }

        public bool IsActionIdValid(string actionId)
        {
            return _agentActions.ContainsKey(actionId);
        }

        public void ProcessAction(string actionId)
        {
            _agentActions[actionId].Process();
        }
    }
}
