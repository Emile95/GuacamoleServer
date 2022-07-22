using GuacamoleAgent.ServerApplication;
using Library.Action;
using Library.Agent.Action;

namespace GuacamoleAgent.Action
{
    public class ClientAgentActionManager : AgentActionManager<AgentAction>
    {
        private readonly ServerClient _serverClient;

        public ClientAgentActionManager(ServerClient serverClient)
        {
            _serverClient = serverClient;
        }

        public override bool AddAgentAction(AgentActionLoaded<AgentAction> agentActionLoaded)
        {
            _agentActionsLoaded.Add(agentActionLoaded.ActionId, agentActionLoaded);
            Console.WriteLine("New action " + agentActionLoaded.DisplayName + " added");
            return true;
        }

        public void ProcessAction(string actionId, string agentActionId)
        {
            _agentActionsLoaded[actionId].Instance.Process();
            _serverClient.FinishAction(agentActionId);
        }
    }
}
