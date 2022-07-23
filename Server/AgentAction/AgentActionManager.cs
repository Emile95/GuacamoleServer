using Server.DataModel;
using API.AgentAction;
using Common;
using Server.Agent;
using API.AgentAction.Configuration;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.Json;

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
            _logger.Log("Added action named " + agentActionLoaded.DisplayName + ", id " + agentActionLoaded.ActionId);
            return agentActionLoaded.ActionId;
        }

        public string ProcessAgentAction(ProcessActionDataModel processActionDataModel)
        {
            if (_agentActionsLoaded.ContainsKey(processActionDataModel.ActionId) == false) throw new Exception("there is no agent action with id : " + processActionDataModel.ActionId);
            AgentActionLoaded agentActionLoaded = _agentActionsLoaded[processActionDataModel.ActionId];
            object agentActionParameter = null;
            if (agentActionLoaded.ParameterType != null)
                agentActionParameter = ResolveAgentActionParameter(agentActionLoaded.ParameterType, processActionDataModel.Parameter);
            
            AgentClient agentClient = _agentManager.GetAvailableAgentByLabel(processActionDataModel.AgentLabel);
            if (agentClient == null) throw new Exception("there is no available agent of label : " + processActionDataModel.AgentLabel);

            string runningAgentActionId = UniqueIdGenerator.Generate(_runningAgentActions.Keys);
            RunningAgentActionLogs runningAgentActionLogs = new RunningAgentActionLogs();
            runningAgentActionLogs.Add(RunningAgentActionLogType.Fatal, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Info, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Error, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Warning, new List<RunningAgentActionLog>());
            runningAgentActionLogs.Add(RunningAgentActionLogType.Succeed, new List<RunningAgentActionLog>());
            _runningAgentActions.Add(runningAgentActionId, runningAgentActionLogs);

            agentClient.ProcessAction(processActionDataModel.ActionId, runningAgentActionId, agentActionParameter);

            _logger.Log("Run agent action " + agentActionLoaded.DisplayName + ", running id : " + runningAgentActionId);

            return runningAgentActionId;
        }

        public void RemoveRunningAction(string runningActionId)
        {
            _runningAgentActions.Remove(runningActionId);
        }

        public void LogRunningAgentAction(RunningAgentActionLog runningAgentActionLog)
        {
            _runningAgentActions[runningAgentActionLog.RunningAgentActionId][runningAgentActionLog.RunningAgentActionLogType].Add(runningAgentActionLog);

            string logType = "";
            switch(runningAgentActionLog.RunningAgentActionLogType)
            {
                case RunningAgentActionLogType.Succeed:
                    _runningAgentActions.Remove(runningAgentActionLog.RunningAgentActionId);
                    logType = "SUCCEED";
                    break;
                case RunningAgentActionLogType.Error:
                    logType = "ERROR";
                    break;
                case RunningAgentActionLogType.Warning:
                    logType = "WARNING";
                    break;
                case RunningAgentActionLogType.Info:
                    logType = "INFO";
                    break;
                case RunningAgentActionLogType.Fatal:
                    _runningAgentActions.Remove(runningAgentActionLog.RunningAgentActionId);
                    logType = "FATAL";
                    break;
            }

            _logger.Log(logType + " -- Running Agent Action '" + runningAgentActionLog.RunningAgentActionId + "', message : " + runningAgentActionLog.Message);
        }

        private string GetNewID()
        {
            return UniqueIdGenerator.Generate(_agentActionsLoaded.Keys);
        }

        private object ResolveAgentActionParameter(Type agentActionParameterType, object parameter)
        {
            if (parameter == null) throw new Exception("need to provide parameter to your action : " + agentActionParameterType.GetProperties().ToString());

            object result = Activator.CreateInstance(agentActionParameterType);

            parameter = JsonConvert.DeserializeObject(((JsonElement)parameter).GetRawText(), agentActionParameterType);

            PropertyInfo[] props = agentActionParameterType.GetProperties();
            foreach(PropertyInfo prop in props)
            {
                ParameterMemberAttriubte parameterMemberAttriubte = prop.GetCustomAttribute<ParameterMemberAttriubte>();
                if (parameterMemberAttriubte == null) continue;
                object propResult = prop.GetValue(parameter);
                if(parameterMemberAttriubte.IsRequired && propResult == null)
                    throw new Exception("Parameter member '" + prop.Name + "' is required");
                if (propResult == null)
                    propResult = parameterMemberAttriubte.DefaultValue;
                prop.SetValue(result, propResult);
            }

            return result;
        }
    }
}
