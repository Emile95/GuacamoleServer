﻿using Library.Configuration.EventHandler;
using Library.Configuration.Http;
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

        public void ResolveAll(ApplicationBase application, ApplicationContext context)
        {
            ResolveEventHandlers(application, context);
            ResolveHttpRequests(application, context);
        }
        public void ResolveEventHandlers(ApplicationBase application, ApplicationContext context)
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

        public void ResolveHttpRequests(ApplicationBase application, ApplicationContext context)
        {
            Type applicationType = application.GetType();
            MethodInfo[] methodInfos = applicationType.GetMethods();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                HttpRequestAttribute attribute = methodInfo.GetCustomAttribute<HttpRequestAttribute>();
                if (attribute == null) continue;
                attribute.Pattern = context.Guid + "/function/" + attribute.Pattern;
                context.HttpRequestAttributes.Add(attribute);
                _httpRequestManager.Add((context) => {
                    methodInfo.Invoke(application, new object[] { context });
                }, attribute);
            }
            application.AddBaseHttpRequests(context, _httpRequestManager);
        }
    }
}
