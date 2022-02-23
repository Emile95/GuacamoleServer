using Library.EventHandler;
using Library.Http;
using System.Reflection;

namespace Library.Application
{
    public class ApplicationResolver
    {
        public void ResolveEventHandlers(EventHandlerManager eventHandlerManager, object application)
        {
            ResolveMethodInfoAttribute(eventHandlerManager, application);
        }

        public void ResolveHttpRequests(HttpRequestManager httpRequestManager, object application)
        {
            ResolveMethodInfoAttribute(httpRequestManager, application);
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
