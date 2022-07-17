using Application.Agent.Request.DataModel;
using Application.DataModel;
using Application.Job;
using System.Net.Sockets;

namespace Application.Agent
{
    public class AgentManager
    {
        private readonly Application.Logger.ILogger _logger;
        private readonly Dictionary<string, AgentClient> _agentClients;
        private readonly Dictionary<string, List<AgentClient>> _agentClientsByLabels;
        private readonly JobManager _jobManager;

        public AgentManager(Application.Logger.ILogger logger, JobManager jobManager)
        {
            _logger = logger;
            _agentClients = new Dictionary<string, AgentClient>();
            _agentClientsByLabels = new Dictionary<string, List<AgentClient>>();
            _jobManager = jobManager;
        }

        public void AddAgent(AgentDefinition agentDefinition, Socket agentSocket)
        {
            AgentClient agentClient = new AgentClient(agentDefinition, agentSocket, _logger, _jobManager);
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

        public object StartJobOnAgent(StartJobDataModel job)
        {
            if (_agentClientsByLabels.ContainsKey(job.AgentLabel) == false) return "No agent with label " + job.AgentLabel;

            AgentClient foundedAgent = null;
            foreach(AgentClient agentClient in _agentClientsByLabels[job.AgentLabel])
            {
                if (agentClient.IsAvailable() == false) continue;
                foundedAgent = agentClient;
                break;
            }

            if (foundedAgent == null) return "There is no available agent for the label : " + job.AgentLabel;

            foundedAgent.RunJob(job);

            return "job started";
        }
    }
}
