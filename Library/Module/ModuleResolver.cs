using Library.Configuration.Module.EventHandler;
using Library.EventHandler;
using System.Reflection;

namespace Library.Module
{
    public class ModuleResolver
    {
        public void ResolveEventHandlers(EventHandlerManager eventHandlerManager, object module)
        {
            Type moduleType = module.GetType();
            MethodInfo[] methodInfos = moduleType.GetMethods();
            foreach(MethodInfo methodInfo in methodInfos)
            {
                IEnumerable<EventHandlerAttribute> eventHandlerAttributes = methodInfo.GetCustomAttributes<EventHandlerAttribute>();
                if (eventHandlerAttributes == null) continue;
                eventHandlerManager.AddEventHandler((context) => { 
                    methodInfo.Invoke(module, new object[] { context });
                }, eventHandlerAttributes);
            }
        }
    }
}
