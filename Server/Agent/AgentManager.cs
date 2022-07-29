using System.Net.Sockets;
using API.Agent;
using API.Logging;

namespace Server.Agent
{
    public class AgentManager
    {
        private readonly AgentLoggers _agentLoggers;
        private readonly Dictionary<string, AgentClient> _agentClients;
        private readonly Dictionary<string, List<AgentClient>> _agentClientsByLabels;

        public AgentManager(AgentLoggers agentLoggers)
        {
            _agentLoggers = agentLoggers;
            _agentClients = new Dictionary<string, AgentClient>();
            _agentClientsByLabels = new Dictionary<string, List<AgentClient>>();
        }

        public AgentClient GetAgentClientById(string id)
        {
            return _agentClients[id];
        }

        public AgentClient AddAgent(string id, AgentDefinition agentDefinition, Action<byte[]> sendToSocketAction)
        {
            AgentClient agentClient = new AgentClient(agentDefinition, _agentLoggers, sendToSocketAction);
            _agentClients.Add(id, agentClient);
            foreach(string label in agentDefinition.Labels)
            {
                if (_agentClientsByLabels.ContainsKey(label) == false)
                    _agentClientsByLabels.Add(label, new List<AgentClient>());
                _agentClientsByLabels[label].Add(agentClient);
            }
            _agentLoggers.Log("Agent " + agentDefinition.Name + " connected, id : " + id);
            return agentClient;
        }

        public void RemoveAgent(string id)
        {
            AgentClient agentClient = _agentClients[id];

            List<string> emptyLabels = new List<string>();
            foreach (string label in agentClient.GetLabels())
            {
                foreach (AgentClient ac in _agentClientsByLabels[label])
                {
                    if (agentClient.GetName().Equals(ac.GetName()))
                    {
                        _agentClientsByLabels[label].Remove(agentClient);
                        break;
                    }
                }
                if (_agentClientsByLabels[label].Count == 0)
                    emptyLabels.Add(label);
            }

            foreach (string label in emptyLabels)
                _agentClientsByLabels.Remove(label);

            _agentClients.Remove(id);
        }

        public void LostUnexpeclyAgentSocket(string id)
        {
            RemoveAgent(id);
            _agentLoggers.Log("Lost unexpectly agent, id : " + id);
        }

        public AgentClient GetAvailableAgentByLabel(string label)
        {
            if (_agentClientsByLabels.ContainsKey(label) == false) throw new Exception("No agent with label " + label);

            AgentClient foundedAgent = null;
            foreach (AgentClient agentClient in _agentClientsByLabels[label])
            {
                if (agentClient.IsAvailable() == false) continue;
                foundedAgent = agentClient;
                break;
            }

            if (foundedAgent == null) throw new Exception("No agent available with label " + label);

            return foundedAgent;
        }
    }
}
