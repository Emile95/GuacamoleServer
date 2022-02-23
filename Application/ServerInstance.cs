using Library.EventHandler;
using Library.Application;
using Library.Http;

namespace App
{
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
            _applicationManager = new ApplicationManager(_applicationResolver);
        }

        public void InitializeApplications()
        {
            _applicationManager.InitializeApplications();
        }

        public Dictionary<HttpRequestType, List<HttpRequestDefinition>> GetHttpRequests()
        {
            return _httpRequestManager.HttpRequest;
        }

        public object RunHttpRequest(HttpRequestDefinition httpRequestDefinition, HttpRequestContext context)
        {
            try
            {
                EventHandlerContext eventHandlerContext = new EventHandlerContext
                {
                    HttpRequestContext = context
                };
                _eventHandlerManager.CallEventHandlers(EventHandlerType.BeforeHttpRequest, eventHandlerContext);
                object response = _httpRequestManager.RunHttpRequest(httpRequestDefinition, context);
                _eventHandlerManager.CallEventHandlers(EventHandlerType.AfterHttpRequest, eventHandlerContext);
                return response;
            } catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
