using Agent.Application;
using Agent.ServerApplication;
using Agent.ServerApplication.Request;
using System.Net.Sockets;
using Agent.AgentAction;
using API.Logging;
using Common.Sockets;
using Agent.Sockets;

namespace Agent
{
    public class AgentInstance
    {
        private readonly SocketLoggers _socketLoggers;
        private readonly SocketRequestLoggers _socketRequestLoggers;
        private readonly AgentActionLoggers _agentActionLoggers;

        private readonly AgentApplicationManager _agentApplicationManager;
        private readonly ClientServerSocketHandler _clientServerSocketHandler;
        private readonly AgentActionManager _agentActionManager;
        private readonly ServerOperations _serverOperations;
        private readonly ServerRequestHandler _serverRequestReceivedHandler;

        public AgentInstance()
        {
            _socketLoggers = new SocketLoggers();
            _socketRequestLoggers = new SocketRequestLoggers();
            _agentActionLoggers = new AgentActionLoggers();

            Socket socket = SocketFactory.CreateSocket(Configuration.ServerProtocol);

            _serverOperations = new ServerOperations();

            _agentActionManager = new AgentActionManager(_agentActionLoggers, _serverOperations);

            _agentApplicationManager = new AgentApplicationManager(_agentActionManager);

            _serverRequestReceivedHandler = new ServerRequestHandler(_socketRequestLoggers, _agentActionManager, _agentApplicationManager);

            _clientServerSocketHandler = new ClientServerSocketHandler(socket, 100000, _serverRequestReceivedHandler, _serverOperations);
        }

        public void StartSocket()
        {
            _clientServerSocketHandler.Initialize((int)Configuration.ServerPort);
        }
    }
}
