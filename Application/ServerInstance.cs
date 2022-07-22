using Application.Agent;
using Application.Logger;
using Application.Agent.Sockets;
using Application.Agent.Request;
using Application.Agent.Action;
using Library.Agent.Action;
using Library;
using Application.Config;
using Application;
using Library.Server.Application;
using Library.Server.EventHandler;

public class ServerInstance
{
    private readonly ServerConfig _config;

    private Library.Logger.ILogger _logger;

    private readonly ServerApplicationManager _applicationManager;
    private readonly ServerApplicationResolver _applicationResolver;
    private readonly ServerEventHandlerManager _eventHandlerManager;

    private WebApplication _webApplication;

    private readonly AgentManager _agentManager;
    private readonly ServerAgentActionManager _serverAgentActionManager;
    private readonly RequestReceivedHandler _agentRequestReceivedHandler;

    private readonly AgentSocketsHandler _agentSocketsHandler;

    public ServerInstance(ServerConfig serverConfig)
    {
        _config = serverConfig;

        _eventHandlerManager = new ServerEventHandlerManager();
        _applicationResolver = new ServerApplicationResolver(_eventHandlerManager);
        _applicationManager = new ServerApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );

        _logger = new ConsoleLogger();

        _agentManager = new AgentManager(_logger);

        _serverAgentActionManager = new ServerAgentActionManager(_logger, _agentManager);

        _agentRequestReceivedHandler = new RequestReceivedHandler(_logger);

        _agentSocketsHandler = AgentSocketsHandlerFactory.CreateAgentSocketsHandler(_config.AgentSocketsConfig, _logger, _agentManager, _serverAgentActionManager, _agentRequestReceivedHandler);

        LoadAgentActionsPlugins();
    }

    public void LoadApplications()
    {
        _applicationManager.LoadApplications();
    }

    public void RunWebApp(string[] args)
    {
        _webApplication = Application.RestAPI.WebApplicationBuilder.BuildWebApplication(_applicationManager, _agentManager, _serverAgentActionManager);
        _webApplication.RunAsync();
    }

    public void StartSockets()
    {
        _agentSocketsHandler.Start();
    }

    private void LoadAgentActionsPlugins()
    {
        string[] agentActionDirectoryPaths = Directory.GetDirectories(ApplicationConstValue.AGENTAPPSPATH);

        foreach (string agentActionDirectoryPath in agentActionDirectoryPaths)
        {
            string[] dlls = Directory.GetFiles(agentActionDirectoryPath, "*.dll");

            foreach (string dll in dlls)
            {
                List<AgentAction> agentActions = PluginFactory.CreatePluginsFromFile<AgentAction>(dll);

                foreach (AgentAction agentAction in agentActions)
                {
                    AgentActionDefinition definition = agentAction.GetAgentActionDefinition();
                    AgentActionLoaded<Tuple<string, byte[]>> agentActionLoaded = new AgentActionLoaded<Tuple<string, byte[]>>();
                    agentActionLoaded.ActionId = _serverAgentActionManager.GetNewID();
                    agentActionLoaded.DisplayName = definition.DisplayName;
                    byte[] dllFile = File.ReadAllBytes(dll);
                    agentActionLoaded.Instance = new Tuple<string, byte[]>(Path.GetFileName(dll), dllFile);
                    _serverAgentActionManager.AddAgentAction(agentActionLoaded);
                }
            }
        }
    }
}

