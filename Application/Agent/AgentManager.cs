using System.Net.Sockets;

namespace Application.Agent
{
    public class AgentManager
    {
        private readonly List<Socket> _agentSockets;

        public AgentManager()
        {
            _agentSockets = new List<Socket>();
        }

        public void AddAgent(Socket agentSocket)
        {
            _agentSockets.Add(agentSocket);
        }

        public void RemoveAgent(Socket agentSocket)
        {
            _agentSockets.Remove(agentSocket);
        }
    }
}
