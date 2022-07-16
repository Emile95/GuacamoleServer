using Library.EventHandler;
using Library.Application;
using Application.Sockets;
using Application.Logger;

public class ServerInstance
{
    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;
    private WebApplication _webApplication;

    private Application.Logger.ILogger _logger;

    private readonly TCPIPSocket _tcpipSocket;

    public ServerInstance()
    {
        _eventHandlerManager = new EventHandlerManager();
        _applicationResolver = new ApplicationResolver(_eventHandlerManager);
        _applicationManager = new ApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );

        SocketDataHandler socketDataHandler = new SocketDataHandler();

        _logger = new ConsoleLogger();

        _tcpipSocket = new TCPIPSocket(_logger, socketDataHandler, 1100);
    }

    public void LoadApplications()
    {
        _applicationManager.LoadApplications();
    }

    public void RunWebApp(string[] args)
    {
        _webApplication = Application.RestAPI.WebApplicationBuilder.BuildWebApplication(_applicationManager);
        _webApplication.RunAsync();
    }

    public void StopWebApp()
    {
        _webApplication.StopAsync();
    }

    public void StartSockets()
    {
        _tcpipSocket.Start();
    }

    public void StopSockets()
    {
        _tcpipSocket.Stop();
    }
}

