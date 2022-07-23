using Server.Agent;
using Server.Logger;
using Server.Agent.Sockets;
using Server.AgentAction;
using Server.Config;
using API.Server.EventHandler;
using Server.Application;

namespace Server
{
    public class ServerInstance
    {
        private readonly ServerConfig _config;

        private API.Logger.ILogger _logger;

        private readonly ServerEventHandlerManager _eventHandlerManager;
        private readonly ServerApplicationResolver _applicationResolver;
        private readonly ServerApplicationManager _serverApplicationManager;

        private readonly AgentManager _agentManager;
        private readonly AgentActionManager _serverAgentActionManager;

        private readonly AgentApplicationManager _agentApplicationManager;

        private readonly AgentRequestHandler _agentRequestReceivedHandler;
        private readonly AgentSocketsHandler _agentSocketsHandler;

        private WebApplication _webApplication;

        public ServerInstance(ServerConfig serverConfig)
        {
            _config = serverConfig;

            _logger = new ConsoleLogger();

            _eventHandlerManager = new ServerEventHandlerManager();
            _applicationResolver = new ServerApplicationResolver(_eventHandlerManager);

            _serverApplicationManager = new ServerApplicationManager(
                _applicationResolver,
                _eventHandlerManager
            );

            _agentManager = new AgentManager(_logger);

            _serverAgentActionManager = new AgentActionManager(_logger, _agentManager);

            _agentApplicationManager = new AgentApplicationManager(_serverAgentActionManager);

            _agentRequestReceivedHandler = new AgentRequestHandler(_logger, _agentApplicationManager, _serverAgentActionManager, _agentManager);

            _agentSocketsHandler = AgentSocketsHandlerFactory.CreateAgentSocketsHandler(_config.AgentSocketsConfig, _logger, _agentRequestReceivedHandler);
        }

        public void LoadServerApplications()
        {
            _serverApplicationManager.LoadApplications();
        }

        public void LoadAgentApplications()
        {
            _agentApplicationManager.LoadApplications();
        }

        public void RunWebApp(string[] args)
        {
            _webApplication = Server.RestAPI.WebApplicationBuilder.BuildWebApplication(_serverApplicationManager, _agentManager, _serverAgentActionManager);
            _webApplication.RunAsync();
        }

        public void StartSockets()
        {
            _agentSocketsHandler.Start();
        }
    }
}



