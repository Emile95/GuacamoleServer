using Library;
using Server.DataModel;
using Library.Action;
using Library.Agent;
using Library.Agent.Action;

namespace Server.Agent.Action
{
    public class ServerAgentActionManager : AgentActionManager<Tuple<string,byte[]>>
    {
        private readonly Library.Logger.ILogger _logger;
        private readonly AgentManager _agentManager;

        protected readonly Dictionary<string, RunningAgentActionLogs> _runningAgentActions;

        public ServerAgentActionManager(Library.Logger.ILogger logger, AgentManager agentManager)
        {
            _logger = logger;
            _agentManager = agentManager;
            _runningAgentActions = new Dictionary<string, RunningAgentActionLogs>();
        }

        public override bool AddAgentAction(AgentActionLoaded<Tuple<string,byte[]>> agentActionLoaded)
        {
            _agentActionsLoaded.Add(agentActionLoaded.ActionId, agentActionLoaded);
            _logger.Log("new action named " + agentActionLoaded.DisplayName + ", id " + agentActionLoaded.ActionId);
            return true;
        }

        public string GetNewID()
        {
            return UniqueIdGenerator.Generate(_agentActionsLoaded.Keys);
        }

        public void ProcessAgentAction(ProcessActionDataModel processActionDataModel)
        {
            AgentClient agentClient = _agentManager.GetAvailableAgentByLabel(processActionDataModel.AgentLabel);
            if (agentClient == null) return;
            if (_agentActionsLoaded.ContainsKey(processActionDataModel.ActionId) == false) return;

            string runningAgentActionId = UniqueIdGenerator.Generate(_runningAgentActions.Keys);
            _runningAgentActions.Add(runningAgentActionId, new RunningAgentActionLogs());

            agentClient.ProcessAction(processActionDataModel.ActionId, runningAgentActionId);
        }

        public void RemoveRunningAction(string runningActionId)
        {
            _runningAgentActions.Remove(runningActionId);
        }
    }
}
