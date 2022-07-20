using Application.Common;
using Application.DataModel;
using Library.Agent;
using Library.Agent.Action;

namespace Application.Agent.Action
{
    public class AgentActionManager
    {
        private readonly Library.Logger.ILogger _logger;

        private readonly Dictionary<string, AgentActionLoaded> _agentActionsLoaded;
        private readonly AgentManager _agentManager;

        public AgentActionManager(Library.Logger.ILogger logger, AgentManager agentManager)
        {
            _logger = logger;
            _agentManager = agentManager;
            _agentActionsLoaded = new Dictionary<string, AgentActionLoaded>();
        }

        public string AddActionLoaded(AgentAction agentAction)
        {
            AgentActionDefinition definition = agentAction.GetAgentActionDefinition();
            string id = UniqueIdGenerator.Generate(_agentActionsLoaded.Keys.ToList());
            AgentActionLoaded agentActionLoaded = new AgentActionLoaded();

            agentActionLoaded.ActionId = id;
            agentActionLoaded.DisplayName = definition.DisplayName;
            agentActionLoaded.Type = agentAction.GetType();

            _agentActionsLoaded.Add(id, agentActionLoaded);

            _logger.Log("new action named " + definition.DisplayName + ", id " + id);

            return id;
        }

        public List<AgentActionLoaded> GetLoadedAgentActions()
        {
            return _agentActionsLoaded.Values.ToList();
        }

        public void ProcessAgentAction(ProcessActionDataModel processActionDataModel)
        {
            AgentClient agentClient = _agentManager.GetAvailableAgentByLabel(processActionDataModel.AgentLabel);
            if (agentClient == null) return;
            if (_agentActionsLoaded.ContainsKey(processActionDataModel.ActionId) == false) return;
            agentClient.ProcessAction(processActionDataModel.ActionId);
        }
    }
}
