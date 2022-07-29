using Agent.Application;
using Agent.Config;
using Agent.ServerApplication;
using Agent.ServerApplication.Request;
using API.Agent;
using System.Net.Sockets;
using Agent.AgentAction;
using API.Logging;
using Common.Sockets;
using Agent.Sockets;
using System.Net;
using System.Diagnostics;

namespace Agent
{
    public class AgentInstance
    {
        private readonly SocketLoggers _socketLoggers;
        private readonly SocketRequestLoggers _socketRequestLoggers;
        private readonly AgentActionLoggers _agentActionLoggers;

        private readonly AgentConfig _config;
        private readonly AgentDefinition _agentDefinition;
        private readonly AgentApplicationManager _agentApplicationManager;
        //private readonly ServerSocketHandler _serverSocketHandler;
        private readonly ClientServerSocketHandler _clientServerSocketHandler;
        private readonly AgentActionManager _agentActionManager;
        private readonly ServerOperations _serverOperations;
        private readonly ServerRequestHandler _serverRequestReceivedHandler;
        //private readonly Socket _serverSocket;

        public AgentInstance(AgentConfig config)
        {
            Debugger.Launch();

            _config = config;

            _agentDefinition = new AgentDefinition
            {
                Id = _config.Id,
                Name = _config.Name,
                Labels = _config.Labels,
                ConcurrentRun = _config.ConcurrentRun
            };

            AgentDefinitionValues.AgentId = _config.Id;

            _socketLoggers = new SocketLoggers();
            _socketRequestLoggers = new SocketRequestLoggers();
            _agentActionLoggers = new AgentActionLoggers();

            //_serverSocket = ServerSocketFactory.CreateServerSocket(_socketLoggers, _config.ServerSocketConfig.Host ,_config.ServerSocketConfig.Protocol, _config.ServerSocketConfig.Port);

            Socket socket = SocketFactory.CreateSocket(_config.ServerSocketConfig.Protocol);

            _serverOperations = new ServerOperations();

            _agentActionManager = new AgentActionManager(_agentActionLoggers, _serverOperations);

            _agentApplicationManager = new AgentApplicationManager(_agentActionManager);

            _serverRequestReceivedHandler = new ServerRequestHandler(_socketRequestLoggers, _agentActionManager, _agentApplicationManager);

            _clientServerSocketHandler = new ClientServerSocketHandler(socket, 100000, _serverRequestReceivedHandler, _agentDefinition, _serverOperations);

            //_serverSocketHandler = new ServerSocketHandler(_serverSocket, _serverRequestReceivedHandler);
        }

        public void StartSocket()
        {
            _clientServerSocketHandler.Initialize(_config.ServerSocketConfig.Port);
            //_serverSocketHandler.Start(_agentDefinition);
        }
    }
}
