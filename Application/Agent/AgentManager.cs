using System.Net.Sockets;

namespace Application.Agent
{
    public class AgentManager
    {
        private readonly Application.Logger.ILogger _logger;
        private readonly Dictionary<string, AgentClient> _agentClients;
        private readonly Dictionary<string, List<AgentClient>> _agentClientsByLabels;

        public AgentManager(Application.Logger.ILogger logger)
        {
            _logger = logger;
            _agentClients = new Dictionary<string, AgentClient>();
            _agentClientsByLabels = new Dictionary<string, List<AgentClient>>();
        }

        public void AddAgent(AgentDefinition agentDefinition, Socket agentSocket)
        {
            AgentClient agentClient = new AgentClient(agentDefinition, agentSocket, _logger);
            _agentClients.Add(agentDefinition.Id, agentClient);
            foreach(string label in agentDefinition.Labels)
            {
                if (_agentClientsByLabels.ContainsKey(label) == false)
                    _agentClientsByLabels.Add(label, new List<AgentClient>());
                _agentClientsByLabels[label].Add(agentClient);
            }
        }

        public void RemoveAgent(string id)
        {
            _agentClients.Remove(id);
        }

        public void LostUnexpeclyAgentSocket(Socket agentSockect)
        {
            foreach(KeyValuePair<string, AgentClient> set in _agentClients)
            {
                if (set.Value.IsEqualBySocket(agentSockect))
                {
                    _logger.Log("Lost unexpectly agent " + set.Value.GetName());
                    _agentClients.Remove(set.Key);
                    return;
                }
            }
        }
    }
}
