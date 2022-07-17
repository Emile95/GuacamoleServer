using Application.DataModel;
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

        public void LostUnexpeclyAgentSocket(Socket agentSockect)
        {
            string agentId = null;
            foreach (KeyValuePair<string, AgentClient> set in _agentClients)
            {
                if(set.Value.IsEqualBySocket(agentSockect))
                    agentId = set.Key;
            }

            if (agentId == null) return;

            RemoveAgent(agentId);
        }

        public object RunJobOnAgent(JobRun jobRun)
        {
            if (_agentClientsByLabels.ContainsKey(jobRun.AgentLabel) == false) return "No agent with label " + jobRun.AgentLabel;

            foreach(AgentClient agentClient in _agentClientsByLabels[jobRun.AgentLabel])
            {
                if (agentClient.IsLocked()) continue;
                agentClient.Lock();
                _logger.Log("run job on label " + jobRun.AgentLabel);
                agentClient.UnLock();
            }

            return "job started";
        }
    }
}
