using Server.Agent;
using Server.Logger;
using Server.Agent.Sockets;
using Server.Agent.Request;
using Server.Agent.Action;
using Library.Agent.Action;
using Library;
using Server.Config;
using Server;
using Library.Server.Application;
using Library.Server.EventHandler;
using Library.Agent.Configuration.Application;
using Library.Agent.Application;
using System.Reflection;
using Library.Agent.Configuration.Application.AgentAction;
using System.Diagnostics;

public class ServerInstance
{
    private readonly ServerConfig _config;

    private Library.Logger.ILogger _logger;

    private readonly ServerApplicationManager _serverApplicationManager;
    private readonly ServerApplicationResolver _applicationResolver;
    private readonly ServerEventHandlerManager _eventHandlerManager;

    private readonly AgentApplicationManager _agentApplicationManager;

    private WebApplication _webApplication;

    private readonly AgentManager _agentManager;
    private readonly ServerAgentActionManager _serverAgentActionManager;
    private readonly RequestReceivedHandler _agentRequestReceivedHandler;

    private readonly AgentSocketsHandler _agentSocketsHandler;

    public ServerInstance(ServerConfig serverConfig)
    {
        _config = serverConfig;

        _agentApplicationManager = new AgentApplicationManager();

        _eventHandlerManager = new ServerEventHandlerManager();
        _applicationResolver = new ServerApplicationResolver(_eventHandlerManager);
        
        _serverApplicationManager = new ServerApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );

        _logger = new ConsoleLogger();

        _agentManager = new AgentManager(_logger);

        _serverAgentActionManager = new ServerAgentActionManager(_logger, _agentManager);

        _agentRequestReceivedHandler = new RequestReceivedHandler(_logger, _agentApplicationManager);

        _agentSocketsHandler = AgentSocketsHandlerFactory.CreateAgentSocketsHandler(_config.AgentSocketsConfig, _logger, _agentManager, _serverAgentActionManager, _agentRequestReceivedHandler);

        LoadAgentApplicationsPlugins();
    }

    public void LoadServerApplications()
    {
        _serverApplicationManager.LoadApplications();
    }

    public void LoadAgentApplications()
    {
        _serverApplicationManager.LoadApplications();
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

    private void LoadAgentApplicationsPlugins()
    {
        string[] agentActionDirectoryPaths = Directory.GetDirectories(ApplicationConstValue.AGENTAPPSPATH);

        foreach (string agentActionDirectoryPath in agentActionDirectoryPaths)
        {
            string[] dlls = Directory.GetFiles(agentActionDirectoryPath, "*.dll");

            foreach (string dll in dlls)
            {
                List<AgentApplicationBase> agentApplications = PluginFactory.CreatePluginsFromFile<AgentApplicationBase>(dll);
                
                foreach (AgentApplicationBase agentApplication in agentApplications)
                {
                    byte[] fileBinary = File.ReadAllBytes(dll);

                    List<string> agentActionIds = new List<string>();

                    MethodInfo[] methods = agentApplication.GetType().GetMethods();
                    foreach(MethodInfo method in methods)
                    {
                        AgentAction agentAction = method.GetCustomAttribute<AgentAction>();
                        if (agentAction == null) continue;
                        agentActionIds.Add(_serverAgentActionManager.AddAgentAction(agentAction));
                    }

                    _agentApplicationManager.AddAgentApplication(dll, fileBinary, agentActionIds);            
                }
            }
        }
    }
}

