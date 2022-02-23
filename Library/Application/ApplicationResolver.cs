using Library.EventHandler;
using Library.Http;
using System.Reflection;

namespace Library.Application
{
    public class ApplicationResolver
    {
        private readonly EventHandlerManager _eventHandlerManager;
        private readonly HttpRequestManager _httpRequestManager;

        public ApplicationResolver(
            EventHandlerManager eventHandlerManager,
            HttpRequestManager httpRequestManager
        )
        {
            _eventHandlerManager = eventHandlerManager;
            _httpRequestManager = httpRequestManager;
        }

        public void ResolveAll(object application)
        {
            ResolveMethodInfoAttribute(_eventHandlerManager, application);
            ResolveMethodInfoAttribute(_httpRequestManager, application);
        }
        public void ResolveEventHandlers(object application)
        {
            ResolveMethodInfoAttribute(_eventHandlerManager, application);
        }

        public void ResolveHttpRequests(object application)
        {
            ResolveMethodInfoAttribute(_httpRequestManager, application);
        }

        private void ResolveMethodInfoAttribute<Context, AttributeType>(IAttributeManager<Action<Context>, AttributeType> attributeManager, object application)
            where AttributeType : Attribute
        {
            Type applicationType = application.GetType();
            MethodInfo[] methodInfos = applicationType.GetMethods();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                IEnumerable<AttributeType> attributes = methodInfo.GetCustomAttributes<AttributeType>();
                if (attributes == null) continue;
                attributeManager.Add((context) => {
                    methodInfo.Invoke(application, new object[] { context });
                }, attributes);
            }
        }
    }
}
