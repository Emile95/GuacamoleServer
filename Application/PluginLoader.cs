using Library.Application;
using System.Reflection;

namespace App
{
    public class PluginLoader
    {
        private readonly string _pluginsDirectoryPath;

        public PluginLoader()
        {
            _pluginsDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "plugins") ;
        }

        public Assembly[] LoadDlls()
        {
            List<Assembly> assemblies = new List<Assembly>();

            string[] filePaths = Directory.GetFiles(_pluginsDirectoryPath);

            foreach (string filePath in filePaths)
                assemblies.Add(Assembly.LoadFile(filePath));

            return assemblies.ToArray();
        }

        public ApplicationBase GetApplicationImplementation(Assembly assembly)
        {
            Type applicationType = typeof(ApplicationBase);
            foreach (Type type in assembly.GetTypes())
                if (applicationType.IsAssignableFrom(type))
                    return Activator.CreateInstance(type) as ApplicationBase;
            return null;
        }
    }
}
