using Library.EventHandler;
using Library.Application;
using Application.Agent;
using Application.Logger;
using Application.Agent.Sockets;
using Application.Agent.Request;
using Application.Agent.Action;

public class ServerInstance
{
    private Library.Logger.ILogger _logger;

    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;

    private WebApplication _webApplication;

    private readonly AgentManager _agentManager;
    private readonly AgentActionManager _agentActionManager;
    private readonly RequestReceivedHandler _agentRequestReceivedHandler;
    private readonly TCPAgentSocketsHandler _tcpAgentSocketsHandler;

    public ServerInstance()
    {
        _eventHandlerManager = new EventHandlerManager();
        _applicationResolver = new ApplicationResolver(_eventHandlerManager);
        _applicationManager = new ApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );

        _logger = new ConsoleLogger();

        _agentManager = new AgentManager(_logger);

        _agentActionManager = new AgentActionManager(_logger, _agentManager);

        _agentActionManager.AddActionLoaded(new ConsoleLogAgentAction());

        _agentRequestReceivedHandler = new RequestReceivedHandler(_logger);
        _tcpAgentSocketsHandler = new TCPAgentSocketsHandler(_logger, 1100, _agentManager, _agentActionManager, _agentRequestReceivedHandler);
    }

    public void LoadApplications()
    {
        _applicationManager.LoadApplications();
    }

    public void RunWebApp(string[] args)
    {
        _webApplication = Application.RestAPI.WebApplicationBuilder.BuildWebApplication(_applicationManager, _agentManager, _agentActionManager);
        _webApplication.RunAsync();
    }

    public void StopWebApp()
    {
        _webApplication.StopAsync();
    }

    public void StartSockets()
    {
        _tcpAgentSocketsHandler.Start();
    }

    public void StopSockets()
    {
        _tcpAgentSocketsHandler.Stop();
    }
}

