﻿using Agent.Application;
using Agent.Config;
using Agent.ServerApplication;
using Agent.ServerApplication.Request;
using API.Agent;
using System.Net.Sockets;
using Agent.AgentAction;
using API.Logging;

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
        private readonly ServerSocketHandler _serverSocketHandler;
        private readonly AgentActionManager _agentActionManager;
        private readonly ServerOperations _serverOperations;
        private readonly ServerRequestHandler _serverRequestReceivedHandler;
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

            AgentDefinitionValues.AgentId = _config.Id;

            _socketLoggers = new SocketLoggers();
            _socketRequestLoggers = new SocketRequestLoggers();
            _agentActionLoggers = new AgentActionLoggers();

            _serverSocket = ServerSocketFactory.CreateServerSocket(_socketLoggers, _config.ServerSocketConfig.Host ,_config.ServerSocketConfig.Protocol, _config.ServerSocketConfig.Port);

            _serverOperations = new ServerOperations(_serverSocket);

            _agentActionManager = new AgentActionManager(_agentActionLoggers, _serverOperations);

            _agentApplicationManager = new AgentApplicationManager(_agentActionManager);

            _serverRequestReceivedHandler = new ServerRequestHandler(_socketRequestLoggers, _agentActionManager, _agentApplicationManager);

            _serverSocketHandler = new ServerSocketHandler(_serverSocket, _serverRequestReceivedHandler);
        }

        public void StartSocket()
        {
            _serverSocketHandler.Start(_agentDefinition);
        }
    }
}
