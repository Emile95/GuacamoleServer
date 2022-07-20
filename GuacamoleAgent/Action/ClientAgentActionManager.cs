using Library.Action;
using Library.Agent.Action;

namespace GuacamoleAgent.Action
{
    public class ClientAgentActionManager : AgentActionManager<AgentAction>
    {
        public override bool AddAgentAction(AgentActionLoaded<AgentAction> agentActionLoaded)
        {
            _agentActionsLoaded.Add(agentActionLoaded.ActionId, agentActionLoaded);
            Console.WriteLine("New action " + agentActionLoaded.DisplayName + " added");
            return true;
        }

        public void ProcessAction(string actionId)
        {
            _agentActionsLoaded[actionId].Instance.Process();
        }
    }
}
