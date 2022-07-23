using Server.DataModel;
using API.AgentAction;
using Common;
using Server.Agent;
using API.Agent.Configuration.Application.AgentAction;
using API.AgentAction;

namespace Server.AgentAction
{
    public class AgentActionManager
    {
        private readonly API.Logger.ILogger _logger;
        private readonly AgentManager _agentManager;

        private readonly Dictionary<string, AgentActionLoaded> _agentActionsLoaded;
        private readonly Dictionary<string, RunningAgentActionLogs> _runningAgentActions;

        public AgentActionManager(API.Logger.ILogger logger, AgentManager agentManager)
        {
            _logger = logger;
            _agentManager = agentManager;
            _runningAgentActions = new Dictionary<string, RunningAgentActionLogs>();
            _agentActionsLoaded = new Dictionary<string, AgentActionLoaded>();
        }

        public string AddAgentAction(AgentActionAttribute agentAction)
        {
            AgentActionLoaded agentActionLoaded = new AgentActionLoaded
            {
                DisplayName = agentAction.DisplayName,
                ActionId = GetNewID(),
                ParameterType = agentAction.ParameterType
            };
            _agentActionsLoaded.Add(agentActionLoaded.ActionId, agentActionLoaded);
            _logger.Log("new action named " + agentActionLoaded.DisplayName + ", id " + agentActionLoaded.ActionId);
            return agentActionLoaded.ActionId;
        }

        public void ProcessAgentAction(ProcessActionDataModel processActionDataModel)
        {
            if (_agentActionsLoaded.ContainsKey(processActionDataModel.ActionId) == false) return;
            AgentActionLoaded agentActionLoaded = _agentActionsLoaded[processActionDataModel.ActionId];
            if (agentActionLoaded.ParameterType != null)
                ValidateAgentActionParameter(agentActionLoaded.ParameterType, processActionDataModel.Parameter);

            AgentClient agentClient = _agentManager.GetAvailableAgentByLabel(processActionDataModel.AgentLabel);
            if (agentClient == null) return;

            string runningAgentActionId = UniqueIdGenerator.Generate(_runningAgentActions.Keys);
            RunningAgentActionLogs runningAgentActionLogs = new RunningAgentActionLogs();
            runningAgentActionLogs.Add(RunningAgentActionLogType.Fatal, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Info, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Error, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Warning, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Succeed, new List<RunningAgentActionLog>());
            _runningAgentActions.Add(runningAgentActionId, runningAgentActionLogs);

            agentClient.ProcessAction(processActionDataModel.ActionId, runningAgentActionId);
        }

        public void RemoveRunningAction(string runningActionId)
        {
            _runningAgentActions.Remove(runningActionId);
        }

        public void LogRunningAgentAction(RunningAgentActionLog runningAgentActionLog)
        {
            _runningAgentActions[runningAgentActionLog.RunningAgentActionId][runningAgentActionLog.RunningAgentActionLogType].Add(runningAgentActionLog);

            if(runningAgentActionLog.RunningAgentActionLogType == RunningAgentActionLogType.Succeed)
            {
                _runningAgentActions.Remove(runningAgentActionLog.RunningAgentActionId);
                _logger.Log(runningAgentActionLog.Message);
            }
        }

        private string GetNewID()
        {
            return UniqueIdGenerator.Generate(_agentActionsLoaded.Keys);
        }

        private void ValidateAgentActionParameter(Type agentActionParameterType, object parameter)
        {
            if (parameter == null) throw new Exception("need to provide parameter to your action : " + agentActionParameterType.GetProperties().ToString());
        }
    }
}
