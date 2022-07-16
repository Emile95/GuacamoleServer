using System.Net.Sockets;

namespace Application.Agent
{
    public class AgentManager
    {
        private readonly Application.Logger.ILogger _logger;
        private readonly Dictionary<string, Socket> _agentSockets;

        public AgentManager(Application.Logger.ILogger logger)
        {
            _logger = logger;
            _agentSockets = new Dictionary<string, Socket>();
        }

        public void AddAgent(string id, Socket agentSocket)
        {
            _agentSockets.Add(id, agentSocket);
        }

        public void RemoveAgent(string id)
        {
            _agentSockets.Remove(id);
        }

        public void LostUnexpeclyAgentSocket(Socket agentSockect)
        {
            foreach(KeyValuePair<string, Socket> set in _agentSockets)
            {
                if (agentSockect.Equals(set.Value))
                {
                    _logger.Log("Lost unexpectly agent id : " + set.Key);
                    _agentSockets.Remove(set.Key);
                    return;
                }
            }
        }
    }
}
