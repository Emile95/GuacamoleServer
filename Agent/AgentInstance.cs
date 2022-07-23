using Agent.Action;
using Agent.Config;
using Agent.ServerApplication;
using Agent.ServerApplication.Request;
using Library.Agent;
using System.Diagnostics;
using System.Net.Sockets;

namespace Agent
{
    public class AgentInstance
    {
        private readonly AgentConfig _config;
        private readonly AgentDefinition _agentDefinition;
        private readonly AgentApplicationManager _agentApplicationManager;
        private readonly ServerSocketHandler _serverSocketHandler;
        private readonly AgentActionManager _agentActionManager;
        private readonly ServerClient _serverClient;
        private readonly ServerRequestReceivedHandler _serverRequestReceivedHandler;
        private readonly Socket _serverSocket;

        public AgentInstance(AgentConfig config)
        {
            _config = config;

            _agentDefinition = new AgentDefinition
            {
                Id = _config.Id,
                Name = _config.Name,
                Labels = _config.Labels,
                ConcurrentRun = _config.ConcurrentRun
            };


            _serverSocket = ServerSocketFactory.CreateServerSocket(_config.ServerSocketConfig.Protocol, _config.ServerSocketConfig.Port);

            _serverClient = new ServerClient(_serverSocket);

            _agentActionManager = new AgentActionManager(_serverClient);

            _agentApplicationManager = new AgentApplicationManager(_agentActionManager);

            _serverRequestReceivedHandler = new ServerRequestReceivedHandler(_agentActionManager, _agentApplicationManager);

            _serverSocketHandler = new ServerSocketHandler(_serverSocket, _serverRequestReceivedHandler);
        }

        public void StartSocket()
        {
            _serverSocketHandler.Start(_agentDefinition);
        }
    }
}
