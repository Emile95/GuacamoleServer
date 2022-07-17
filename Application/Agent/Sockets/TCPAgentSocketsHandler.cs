﻿using Application.Agent.Request.Received;
using System.Net;
using System.Net.Sockets;

namespace Application.Agent.Sockets
{
    public class TCPAgentSocketsHandler : AgentSocketsHandler
    {
        public TCPAgentSocketsHandler(Application.Logger.ILogger logger, int port, AgentManager agentManager, RequestReceivedHandler agentRequestReceivedHandler)
        : base(logger, port, agentManager, agentRequestReceivedHandler) {}

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
