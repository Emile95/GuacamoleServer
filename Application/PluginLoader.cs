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

        public List<T> GetImplementations<T>(Assembly assembly)
            where T : class
        {
            List<T> list = new List<T>();
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    T result = Activator.CreateInstance(type) as T;
                    if(result != null)
                        list.Add(result);
                }
            }
            return list;
        }

        public List<Type> GetImplementationTypes<T>(Assembly assembly) where T : class
        {
            List<Type> list = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(T).IsAssignableFrom(type))
                    list.Add(type);
            }
            return list;
        }
    }
}
