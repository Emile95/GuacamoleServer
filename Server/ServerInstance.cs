using Server.Agent;
using Server.Logger;
using Server.Agent.Sockets;
using Server.AgentAction;
using Server.Config;
using API.Server.EventHandler;
using Server.Application;
using Server.RestAPI;
using Server.DataModel;

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
        private readonly AgentActionManager _agentActionManager;

        private readonly AgentApplicationManager _agentApplicationManager;

        private readonly AgentRequestHandler _agentRequestReceivedHandler;
        private readonly AgentSocketsHandler _agentSocketsHandler;

        private WebApplication _webApplication;

        private readonly RestAPIHandler _restAPIHandler;

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

            _agentActionManager = new AgentActionManager(_logger, _agentManager);

            _agentApplicationManager = new AgentApplicationManager(_agentActionManager);

            _agentRequestReceivedHandler = new AgentRequestHandler(_logger, _agentApplicationManager, _agentActionManager, _agentManager);

            _agentSocketsHandler = AgentSocketsHandlerFactory.CreateAgentSocketsHandler(_config.AgentSocketsConfig, _logger, _agentManager, _agentRequestReceivedHandler);

            _restAPIHandler = new RestAPIHandler();
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
            _restAPIHandler.Run();
        }

        public void StopWebApp()
        {
            _restAPIHandler.Stop();
        }

        public void StartSockets()
        {
            _agentSocketsHandler.Start();
        }

        public void MapRestAPIRequest()
        {
            _restAPIHandler.MapPost<ProcessActionDataModel>("action/process", (body) => _agentActionManager.ProcessAgentAction(body));
        }
    }
}



