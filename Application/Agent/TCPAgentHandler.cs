using System.Net;
using System.Net.Sockets;

namespace Application.Agent
{
    public class TCPAgentHandler : AgentHandler
    {
        public TCPAgentHandler(Application.Logger.ILogger logger, SocketDataHandler socketDataHandler, int port)
        : base(logger, socketDataHandler, port) {}

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
