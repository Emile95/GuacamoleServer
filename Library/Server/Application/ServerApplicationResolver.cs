﻿using Library.Server.Configuration.EventHandler;
using Library.Server.EventHandler;
using System.Reflection;

namespace Library.Server.Application
{
    public class ServerApplicationResolver
    {
        private readonly ServerEventHandlerManager _eventHandlerManager;

        public ServerApplicationResolver(
            ServerEventHandlerManager eventHandlerManager
        )
        {
            _eventHandlerManager = eventHandlerManager;
        }

        public void ResolveAll(ServerApplicationBase application)
        {
            ResolveEventHandlers(application);
        }
        public void ResolveEventHandlers(ServerApplicationBase application)
        {
            Type applicationType = application.GetType();
            MethodInfo[] methodInfos = applicationType.GetMethods();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                IEnumerable<ServerEventHandlerAttribute> attributes = methodInfo.GetCustomAttributes<ServerEventHandlerAttribute>();
                if (attributes == null) continue;
                _eventHandlerManager.Add((context) => {
                    methodInfo.Invoke(application, new object[] { context });
                }, attributes);
            }
        }
    }
}