using Library.Agent.Application;
using Library.Agent.Configuration.EventHandler;
using Library.Agent.EventHandler;
using System.Reflection;

namespace Agent.Application
{
    public class AgentApplicationResolver
    {
        private readonly AgentEventHandlerManager _eventHandlerManager;

        public AgentApplicationResolver(
            AgentEventHandlerManager eventHandlerManager
        )
        {
            _eventHandlerManager = eventHandlerManager;
        }

        public void ResolveAll(AgentApplicationBase application)
        {
            ResolveEventHandlers(application);
        }
        public void ResolveEventHandlers(AgentApplicationBase application)
        {
            Type applicationType = application.GetType();
            MethodInfo[] methodInfos = applicationType.GetMethods();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                IEnumerable<AgentEventHandlerAttribute> attributes = methodInfo.GetCustomAttributes<AgentEventHandlerAttribute>();
                if (attributes == null) continue;
                _eventHandlerManager.Add((context) => {
                    methodInfo.Invoke(application, new object[] { context });
                }, attributes);
            }
        }
    }
}
