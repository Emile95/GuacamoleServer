using Server.Agent;
using Server.Agent.Sockets;
using Server.AgentAction;
using Server.Config;
using API.Server.EventHandler;
using Server.Application;
using Server.RestAPI;
using Server.DataModel;
using API.Logging;

namespace Server
{
    public class ServerInstance
    {
        private readonly ServerConfig _config;

        private readonly SocketLoggers _socketLoggers;
        private readonly SocketRequestLoggers _socketRequestLoggers;
        private readonly HttpRequestLoggers _httpRequestLoggers;
        private readonly AgentLoggers _agentLoggers;
        private readonly AgentActionLoggers _agentActionLoggers;

        private readonly ServerEventHandlerManager _eventHandlerManager;
        private readonly ServerApplicationResolver _applicationResolver;
        private readonly ServerApplicationManager _serverApplicationManager;

        private readonly AgentManager _agentManager;
        private readonly AgentActionManager _agentActionManager;

        private readonly AgentApplicationManager _agentApplicationManager;

        private readonly AgentRequestHandler _agentRequestReceivedHandler;
        private readonly AgentSocketsHandler _agentSocketsHandler;

        private readonly RestAPIHandler _restAPIHandler;

        public ServerInstance(ServerConfig serverConfig)
        {
            _config = serverConfig;

            _socketLoggers = new SocketLoggers();
            _socketRequestLoggers = new SocketRequestLoggers();
            _httpRequestLoggers = new HttpRequestLoggers();
            _agentLoggers = new AgentLoggers();
            _agentActionLoggers = new AgentActionLoggers();

            _eventHandlerManager = new ServerEventHandlerManager();
            _applicationResolver = new ServerApplicationResolver(_eventHandlerManager);

            _serverApplicationManager = new ServerApplicationManager(
                _applicationResolver,
                _eventHandlerManager
            );

            _agentManager = new AgentManager(_agentLoggers);

            _agentActionManager = new AgentActionManager(_agentActionLoggers, _agentManager);

            _agentApplicationManager = new AgentApplicationManager(_agentActionManager);

            _agentRequestReceivedHandler = new AgentRequestHandler(_socketRequestLoggers, _agentApplicationManager, _agentActionManager, _agentManager);

            _agentSocketsHandler = AgentSocketsHandlerFactory.CreateAgentSocketsHandler(_config.AgentSocketsConfig, _socketLoggers, _agentManager, _agentRequestReceivedHandler);

            _restAPIHandler = new RestAPIHandler(_httpRequestLoggers, _config.WebPort);
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



