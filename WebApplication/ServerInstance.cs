using Library.EventHandler;
using Library.Application;
using Library.Http;

public class ServerInstance
{
    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;
    private readonly HttpRequestManager _httpRequestManager;
        
    public ServerInstance()
    {
        _eventHandlerManager = new EventHandlerManager();
        _httpRequestManager = new HttpRequestManager();
        _applicationResolver = new ApplicationResolver(_eventHandlerManager, _httpRequestManager);
        _applicationManager = new ApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );
    }

    public void LoadApplications()
    {
        _applicationManager.LoadApplications();
    }

    public Dictionary<HttpRequestType, List<HttpRequestDefinition>> GetHttpRequests()
    {
        return _httpRequestManager.HttpRequest;
    }

    public void RunHttpRequest(HttpRequestDefinition httpRequestDefinition, HttpRequestContext context)
    {
        try
        {
            EventHandlerContext eventHandlerContext = new EventHandlerContext
            {
                HttpRequestContext = context
            };
            _eventHandlerManager.CallEventHandlers(EventHandlerType.BeforeHttpRequest, eventHandlerContext);
            _httpRequestManager.RunHttpRequest(httpRequestDefinition, context);
            _eventHandlerManager.CallEventHandlers(EventHandlerType.AfterHttpRequest, eventHandlerContext);
        } catch(Exception ex)
        {
            context.ResponseBody = ex.Message;
        }
    }

    public object InstallPlugin(string path)
    {
        _applicationManager.InstallApplication(path);
        return "plugin installed";
    }

    public object GetApplicationDescriptives()
    {
        return _applicationManager.GetApplicationsDescriptive();
    }
}

