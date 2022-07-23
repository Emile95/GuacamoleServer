using Library.Server.EventHandler;
using Library.Server.Application;
using System.Reflection;

namespace Server.Application
{
    public class ServerApplicationManager
    {
        private readonly Dictionary<string, ServerApplicationBase> _applications;

        private readonly ServerApplicationResolver _applicationResolver;
        private readonly ServerEventHandlerManager _eventHandlerManager;

        public ServerApplicationManager(
            ServerApplicationResolver applicationResolver,
            ServerEventHandlerManager eventHandlerManager
        )
        {
            _applicationResolver = applicationResolver;
            _eventHandlerManager = eventHandlerManager;
            _applications = new Dictionary<string, ServerApplicationBase>();
        }

        public void InstallApplication(string applicationPath)
        {
            FileInfo applicationFileInfo = new FileInfo(applicationPath);
            string newGuid = GetNewGuid();
            string newApplicationDirectoryPath = Path.Combine(ServerApplicationContext.ParentDirectoryPath, newGuid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationPath = Path.Combine(newApplicationDirectoryPath, applicationFileInfo.Name);
            File.Copy(applicationPath, newApplicationPath);
            ServerApplicationBase application = GetApplicationImplementation(Assembly.LoadFile(newApplicationPath));
            application.EventHandlerManager = _eventHandlerManager;
            using (var context = new ServerApplicationContext(newGuid))
            {
                application.Install();
            }
        }

        public void LoadApplications()
        {
            string[] directoryPaths = Directory.GetDirectories(ServerApplicationContext.ParentDirectoryPath);

            foreach (string directoryPath in directoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                string path = Directory.GetFiles(directoryPath, "*.dll")[0];
                ServerApplicationBase application = null;
                try
                {
                    application = GetApplicationImplementation(Assembly.LoadFile(path));
                }
                catch (Exception e) { }
                application.EventHandlerManager = _eventHandlerManager;

                using (var context = new ServerApplicationContext(directoryInfo.Name))
                {
                    application.Initialize();
                    _applications.Add(directoryInfo.Name, application);
                    _applicationResolver.ResolveAll(application);
                }
            }
        }

        public void InitializeApplication(string guid)
        {
            using (var context = new ServerApplicationContext(guid))
            {
                _applications[guid].Initialize();
            }
        }

        public void UninitializeApplication(string guid)
        {
            using (var context = new ServerApplicationContext(guid))
            {
                _applications[guid].Uninitialize();
            }
        }

        public List<object> GetApplicationsDescriptive()
        {
            List<object> applications = new List<object>();

            foreach(KeyValuePair<string, ServerApplicationBase> set in _applications)
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

        private ServerApplicationBase GetApplicationImplementation(Assembly assembly)
        {
            ServerApplicationBase app = null;
            Type applicationType = typeof(ServerApplicationBase);

            foreach (Type type in assembly.GetTypes())
            {
                if (applicationType.IsAssignableFrom(type))
                {
                    app = Activator.CreateInstance(type) as ServerApplicationBase;
                }
            }
                
            return app;
        }

    }
}
