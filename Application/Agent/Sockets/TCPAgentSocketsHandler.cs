using Application.Agent.Request;
using Application.Job;
using System.Net;
using System.Net.Sockets;

namespace Application.Agent.Sockets
{
    public class TCPAgentSocketsHandler : AgentSocketsHandler
    {
        public TCPAgentSocketsHandler(Library.Logger.ILogger logger, int port, AgentManager agentManager, RequestReceivedHandler agentRequestReceivedHandler, JobManager jobManager)
        : base(logger, port, agentManager, agentRequestReceivedHandler, jobManager) {}

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
