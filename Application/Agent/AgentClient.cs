using System.Net.Sockets;

namespace Application.Agent
{
    public class AgentClient
    {
        private readonly AgentDefinition _agentDefinition;
        private readonly Socket _socket;
        private readonly Application.Logger.ILogger _logger;

        public AgentClient(AgentDefinition agentDefinition, Socket socket, Application.Logger.ILogger logger)
        {
            _agentDefinition = agentDefinition;
            _socket = socket;
            _logger = logger;
        }

        public bool IsEqualBySocket(Socket socket)
        {
            return _socket.Equals(socket);
        }

        public string GetName()
        {
            return _agentDefinition.Name;
        }
    }
}
