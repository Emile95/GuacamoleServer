using Application.Common;
using Library.Agent.Action;
namespace Application.Agent.Action
{
    public class AgentActionManager
    {
        private readonly Library.Logger.ILogger _logger;

        private readonly Dictionary<string, AgentActionLoaded> _agentActionsLoaded;

        public AgentActionManager(Library.Logger.ILogger logger)
        {
            _logger = logger;
            _agentActionsLoaded = new Dictionary<string, AgentActionLoaded>();
        }

        public string AddActionLoaded(AgentAction agentAction)
        {
            AgentActionDefinition definition = agentAction.GetAgentActionDefinition();
            string id = UniqueIdGenerator.Generate(_agentActionsLoaded.Keys.ToList());
            AgentActionLoaded agentActionLoaded = new AgentActionLoaded();

            agentActionLoaded.ActionId = id;
            agentActionLoaded.DisplayName = definition.DisplayName;
            agentActionLoaded.AgentAction = agentAction;

            _agentActionsLoaded.Add(id, agentActionLoaded);

            _logger.Log("new action named " + definition.DisplayName + ", id " + id);

            return id;
        }

        public List<AgentActionLoaded> GetLoadedAgentActions()
        {
            return _agentActionsLoaded.Values.ToList();
        }
    }
}
