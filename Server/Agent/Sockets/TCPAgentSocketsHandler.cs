using API.Logging;
using System.Net;
using System.Net.Sockets;

namespace Server.Agent.Sockets
{
    public class TCPAgentSocketsHandler : AgentSocketsHandler
    {
        public TCPAgentSocketsHandler(SocketLoggers socketLoggers, int port, AgentManager agentManager ,AgentRequestHandler agentRequestReceivedHandler)
        : base(socketLoggers, port, agentManager, agentRequestReceivedHandler) {}

        protected override Socket GetSocket()
        {
            return new Socket(
                _hostIpAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );
        }

        protected override EndPoint GetEndpoint()
        {
            return new IPEndPoint(_hostIpAddress, _port);
        }
    }
}
