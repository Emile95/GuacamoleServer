using System.Reflection;

namespace Library.Application
{
    public class ApplicationManager
    {
        private readonly ApplicationResolver _applicationResolver;
        private readonly Dictionary<string, ApplicationBase> _applications;
        private readonly string _applicationPaths;

        public ApplicationManager(ApplicationResolver applicationResolver)
        {
            _applicationResolver = applicationResolver;
            _applications = new Dictionary<string, ApplicationBase>();
            _applicationPaths = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App");
        }

        /*public void InstallApplication(string applicationPath)
        {
            FileInfo applicationFileInfo = new FileInfo(applicationPath);
            string newGuid = GetNewGuid();
            string newApplicationDirectoryPath = Path.Combine(_applicationPaths, "App", newGuid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationPath = Path.Combine(newApplicationDirectoryPath, applicationFileInfo.Name, applicationFileInfo.Extension);
            File.Copy(applicationPath, newApplicationPath);
            ApplicationBase application = GetApplicationImplementation(Assembly.LoadFile(newApplicationPath));
            _applications.Add(newGuid, application);
        }*/

        public void InitializeApplications()
        {
            string[] directoryPaths = Directory.GetDirectories(_applicationPaths);

            foreach (string directoryPath in directoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                string path = Directory.GetFiles(directoryPath, "*.dll")[0];
                ApplicationBase application = GetApplicationImplementation(Assembly.LoadFile(path));
                _applications.Add(directoryInfo.Name, application);
                _applicationResolver.ResolveAll(application);
            }
        }

        private string GetNewGuid()
        {
            string guid;
            do
            {
                guid = Guid.NewGuid().ToString();
            }
            while (_applications.ContainsKey(guid));
            return guid;
        }

        private ApplicationBase GetApplicationImplementation(Assembly assembly)
        {
            Type applicationType = typeof(ApplicationBase);
            foreach (Type type in assembly.GetTypes())
                if (applicationType.IsAssignableFrom(type))
                    return Activator.CreateInstance(type) as ApplicationBase;
            return null;
        }
    }
}
