using Library.EventHandler;
using System.Reflection;

namespace Library.Application
{
    public class ApplicationManager
    {
        private readonly Dictionary<string, ApplicationBase> _applications;

        private readonly ApplicationResolver _applicationResolver;
        private readonly EventHandlerManager _eventHandlerManager;

        public ApplicationManager(
            ApplicationResolver applicationResolver,
            EventHandlerManager eventHandlerManager
        )
        {
            _applicationResolver = applicationResolver;
            _eventHandlerManager = eventHandlerManager;
            _applications = new Dictionary<string, ApplicationBase>();
        }

        public void InstallApplication(string applicationPath)
        {
            FileInfo applicationFileInfo = new FileInfo(applicationPath);
            string newGuid = GetNewGuid();
            string newApplicationDirectoryPath = Path.Combine(ApplicationContext.ParentDirectoryPath, newGuid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationPath = Path.Combine(newApplicationDirectoryPath, applicationFileInfo.Name);
            File.Copy(applicationPath, newApplicationPath);
            ApplicationBase application = GetApplicationImplementation(Assembly.LoadFile(newApplicationPath));
            application.EventHandlerManager = _eventHandlerManager;
            using (var context = new ApplicationContext(newGuid))
            {
                application.Install();
            }
        }

        public void LoadApplications()
        {
            string[] directoryPaths = Directory.GetDirectories(ApplicationContext.ParentDirectoryPath);

            foreach (string directoryPath in directoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                string path = Directory.GetFiles(directoryPath, "*.dll")[0];
                ApplicationBase application = GetApplicationImplementation(Assembly.LoadFile(path));
                application.EventHandlerManager = _eventHandlerManager;

                using (var context = new ApplicationContext(directoryInfo.Name))
                {
                    application.Initialize();
                    _applications.Add(directoryInfo.Name, application);
                    _applicationResolver.ResolveAll(application);
                }
            }
        }

        public void InitializeApplication(string guid)
        {
            using (var context = new ApplicationContext(guid))
            {
                _applications[guid].Initialize();
            }
        }

        public void UninitializeApplication(string guid)
        {
            using (var context = new ApplicationContext(guid))
            {
                _applications[guid].Uninitialize();
            }
        }

        public List<object> GetApplicationsDescriptive()
        {
            List<object> applications = new List<object>();

            foreach(KeyValuePair<string, ApplicationBase> set in _applications)
            {
                applications.Add(
                    new
                    {
                        guid = set.Key,
                        name = set.Value.GetName(),
                        description = set.Value.GetDescription()
                    }
                );
            }

            return applications;
        }

        public bool IsValidGuid(string guid)
        {
            return _applications.ContainsKey(guid);
        }

        private string GetNewGuid()
        {
            string guid;
            do { guid = Guid.NewGuid().ToString(); }
            while (_applications.ContainsKey(guid));
            return guid;
        }

        private ApplicationBase GetApplicationImplementation(Assembly assembly)
        {
            ApplicationBase app = null;
            Type applicationType = typeof(ApplicationBase);
            foreach (Type type in assembly.GetTypes())
            {
                if (applicationType.IsAssignableFrom(type))
                    app = Activator.CreateInstance(type) as ApplicationBase;
            }
                
            return app;
        }

    }
}
