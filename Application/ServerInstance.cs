using Library.EventHandler;
using Library.Application;
using Application.Agent;
using Application.Logger;
using Application.Agent.Sockets;
using Application.Agent.Request.Received;

public class ServerInstance
{
    private Application.Logger.ILogger _logger;

    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;

    private WebApplication _webApplication;

    private readonly AgentManager _agentManager;
    private readonly AgentRequestReceivedHandler _agentRequestReceivedHandler;
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

        _agentRequestReceivedHandler = new AgentRequestReceivedHandler(_logger);

        _tcpAgentSocketsHandler = new TCPAgentSocketsHandler(_logger, 1100, _agentManager, _agentRequestReceivedHandler);
    }

    public void LoadApplications()
    {
        _applicationManager.LoadApplications();
    }

    public void RunWebApp(string[] args)
    {
        _webApplication = Application.RestAPI.WebApplicationBuilder.BuildWebApplication(_applicationManager, _agentManager);
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

