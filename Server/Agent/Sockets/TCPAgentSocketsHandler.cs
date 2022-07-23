using Server.Agent.Request;
using System.Net;
using System.Net.Sockets;

namespace Server.Agent.Sockets
{
    public class TCPAgentSocketsHandler : AgentSocketsHandler
    {
        public TCPAgentSocketsHandler(Library.Logger.ILogger logger, int port, RequestReceivedHandler agentRequestReceivedHandler)
        : base(logger, port, agentRequestReceivedHandler) {}

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
