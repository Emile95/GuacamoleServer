﻿using GuacamoleAgent.Action;
using GuacamoleAgent.Config;
using GuacamoleAgent.ServerApplication;
using GuacamoleAgent.ServerApplication.Request;
using Library.Agent;
using System.Net;
using System.Net.Sockets;

namespace GuacamoleAgent
{
    public class AgentInstance
    {
        private readonly AgentConfig _config;
        private readonly AgentDefinition _agentDefinition;

        private readonly ServerSocketHandler _serverSocketHandler;
        private readonly ClientAgentActionManager _clientAgentActionManager;
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

            _clientAgentActionManager = new ClientAgentActionManager(_serverClient);

            _serverRequestReceivedHandler = new ServerRequestReceivedHandler(_clientAgentActionManager);

            _serverSocketHandler = new ServerSocketHandler(_serverSocket, _serverRequestReceivedHandler);
        }

        public void StartSocket()
        {
            _serverSocketHandler.Start(_agentDefinition);
        }
    }
}
