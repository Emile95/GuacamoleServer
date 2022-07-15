using Library.EventHandler;
using Library.Application;

public class ServerInstance
{
    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;
    private WebApplication _webApplication;

    public ServerInstance()
    {
        _eventHandlerManager = new EventHandlerManager();
        _applicationResolver = new ApplicationResolver(_eventHandlerManager);
        _applicationManager = new ApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );

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
}

