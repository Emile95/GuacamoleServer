using Application.Common;
using Application.DataModel;
using Library.Action;
using Library.Agent;
using Library.Agent.Action;

namespace Application.Agent.Action
{
    public class ServerAgentActionManager : AgentActionManager<Tuple<string,byte[]>>
    {
        private readonly Library.Logger.ILogger _logger;
        private readonly AgentManager _agentManager;

        public ServerAgentActionManager(Library.Logger.ILogger logger, AgentManager agentManager)
        {
            _logger = logger;
            _agentManager = agentManager;
        }

        public override bool AddAgentAction(AgentActionLoaded<Tuple<string,byte[]>> agentActionLoaded)
        {
            _agentActionsLoaded.Add(agentActionLoaded.ActionId, agentActionLoaded);
            _logger.Log("new action named " + agentActionLoaded.DisplayName + ", id " + agentActionLoaded.ActionId);
            return true;
        }

        public string GetNewID()
        {
            return UniqueIdGenerator.Generate(_agentActionsLoaded.Keys.ToList());
        }

        public void ProcessAgentAction(ProcessActionDataModel processActionDataModel)
        {

            /*AgentClient agentClient = _agentManager.GetAvailableAgentByLabel(processActionDataModel.AgentLabel);
            if (agentClient == null) return;
            if (_agentActionsLoaded.ContainsKey(processActionDataModel.ActionId) == false) return;
            agentClient.ProcessAction(processActionDataModel.ActionId);*/
        }
    }
}
