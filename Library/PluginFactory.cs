using System.Reflection;

namespace Library
{
    public static class PluginFactory
    {
        public static List<PluginType> CreatePluginsFromFile<PluginType>(string dllPath)
        where PluginType : class
        {
            List<PluginType> plugins = new List<PluginType>();
            Assembly jihogoAssembly = Assembly.LoadFile(dllPath);

            PluginType jihoAgentAction = null;
            Type applicationType = typeof(PluginType);

            foreach (Type type in jihogoAssembly.GetTypes())
            {
                if (applicationType.IsAssignableFrom(type))
                    plugins.Add(Activator.CreateInstance(type) as PluginType); 
            }

            return plugins;
        }
    }
}
