using Library.EventHandler;
using Library.Application;
using Application.Agent;
using Application.Logger;
using Application.Agent.Sockets;
using Application.Agent.Request;
using Application.Agent.Action;
using Library.Agent.Action;
using Library;
using Application.Config;

public class ServerInstance
{
    private readonly ServerConfig _config;

    private Library.Logger.ILogger _logger;

    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;

    private WebApplication _webApplication;

    private readonly AgentManager _agentManager;
    private readonly ServerAgentActionManager _serverAgentActionManager;
    private readonly RequestReceivedHandler _agentRequestReceivedHandler;

    private readonly AgentSocketsHandler _agentSocketsHandler;

    public ServerInstance(ServerConfig serverConfig)
    {
        _config = serverConfig;

        _eventHandlerManager = new EventHandlerManager();
        _applicationResolver = new ApplicationResolver(_eventHandlerManager);
        _applicationManager = new ApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );

        _logger = new ConsoleLogger();

        _agentManager = new AgentManager(_logger);

        _serverAgentActionManager = new ServerAgentActionManager(_logger, _agentManager);

        string [] agentActionDirectoryPaths = Directory.GetDirectories(ApplicationConstValue.AGENTAPPSPATH);

        foreach(string agentActionDirectoryPath in agentActionDirectoryPaths)
        {
            string[] dlls = Directory.GetFiles(agentActionDirectoryPath, "*.dll");

            foreach(string dll in dlls)
            {
                List<AgentAction> agentActions = PluginFactory.CreatePluginsFromFile<AgentAction>(dll);

                foreach(AgentAction agentAction in agentActions)
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

        _agentRequestReceivedHandler = new RequestReceivedHandler(_logger);

        _agentSocketsHandler = AgentSocketsHandlerFactory.CreateAgentSocketsHandler(_config.AgentSocketConfig, _logger, _agentManager, _serverAgentActionManager, _agentRequestReceivedHandler);
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

    public void StopWebApp()
    {
        _webApplication.StopAsync();
    }

    public void StartSockets()
    {
        _agentSocketsHandler.Start();
    }

    public void StopSockets()
    {
        _agentSocketsHandler.Stop();
    }
}

