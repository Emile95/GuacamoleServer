using System.Reflection;
using System.Text.Json;
using App.Exceptions;
using Library;
using Library.EventHandler;
using Library.Logger;
using Library.Application;
using Library.Http;

namespace App
{
    public class ServerInstance
    {
        private readonly PluginLoader _pluginLoader;
        private readonly List<ApplicationBase> _applications;
        private readonly ApplicationResolver _applicationResolver;
        private readonly EventHandlerManager _eventHandlerManager;
        private readonly HttpRequestManager _httpRequestManager;
        
        public ServerInstance()
        {
            _pluginLoader = new PluginLoader();
            _applications = new List<ApplicationBase>();
            _applicationResolver = new ApplicationResolver();
            _eventHandlerManager = new EventHandlerManager();
            _httpRequestManager = new HttpRequestManager();
        }

        public void LoadPlugins()
        {
            Assembly[] assemblies = _pluginLoader.LoadDlls();

            foreach(Assembly assembly in assemblies)
                _applications.AddRange(_pluginLoader.GetImplementations<ApplicationBase>(assembly));
            
            for(int i = 0; i < _applications.Count; i++)
            {
                ApplicationBase app = _applications[i];
                app.EventHandlerManager = _eventHandlerManager;
                _applicationResolver.ResolveEventHandlers(_eventHandlerManager, app);
                _applicationResolver.ResolveHttpRequests(_httpRequestManager, app);
            }
        }

        public Dictionary<HttpRequestType, List<HttpRequestDefinition>> GetHttpRequests()
        {
            return _httpRequestManager.HttpRequest;
        }

        public object RunHttpRequest(HttpRequestDefinition httpRequestDefinition, HttpRequestContext context)
        {
            context.ExpectedBody = httpRequestDefinition.ExcpectedType;
            return _httpRequestManager.RunHttpRequest(httpRequestDefinition.Request, context);
        }
    }
}
