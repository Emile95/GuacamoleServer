using Library.Configuration.EventHandler;
using Library.EventHandler;
using System.Reflection;

namespace Library.Application
{
    public class ApplicationResolver
    {
        private readonly EventHandlerManager _eventHandlerManager;

        public ApplicationResolver(
            EventHandlerManager eventHandlerManager
        )
        {
            _eventHandlerManager = eventHandlerManager;
        }

        public void ResolveAll(ApplicationBase application)
        {
            ResolveEventHandlers(application);
        }
        public void ResolveEventHandlers(ApplicationBase application)
        {
            Type applicationType = application.GetType();
            MethodInfo[] methodInfos = applicationType.GetMethods();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                IEnumerable<EventHandlerAttribute> attributes = methodInfo.GetCustomAttributes<EventHandlerAttribute>();
                if (attributes == null) continue;
                _eventHandlerManager.Add((context) => {
                    methodInfo.Invoke(application, new object[] { context });
                }, attributes);
            }
        }
    }
}
