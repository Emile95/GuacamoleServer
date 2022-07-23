using Agent.Action;
using Library.Agent.Application;
using Library.Agent.Configuration.Application.AgentAction;
using Library.Agent.EventHandler;
using System.Reflection;

namespace Agent.Application
{
    public class AgentApplicationManager
    {
        private readonly Dictionary<string, AgentApplicationBase> _applications;

        private readonly AgentApplicationResolver _applicationResolver;
        private readonly AgentEventHandlerManager _eventHandlerManager;
        private readonly AgentActionManager _agentActionManager;

        public AgentApplicationManager(
            //AgentApplicationResolver applicationResolver,
            //AgentEventHandlerManager eventHandlerManager
            AgentActionManager agentActionManager
        )
        {
            //_applicationResolver = applicationResolver;
            //_eventHandlerManager = eventHandlerManager;
            _applications = new Dictionary<string, AgentApplicationBase>();
            _agentActionManager = agentActionManager;
        }

        public void InstallApplication(string applicationPath, List<string> agentActionIds)
        {
            FileInfo applicationFileInfo = new FileInfo(applicationPath);
            string newGuid = GetNewGuid();
            string newApplicationDirectoryPath = Path.Combine(AgentApplicationContext.ParentDirectoryPath, newGuid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationPath = Path.Combine(newApplicationDirectoryPath, applicationFileInfo.Name);
            File.Copy(applicationPath, newApplicationPath);
            AgentApplicationBase application = GetApplicationImplementation(Assembly.LoadFile(newApplicationPath));
            application.EventHandlerManager = _eventHandlerManager;
            using (var context = new AgentApplicationContext(newGuid))
            {
                application.Install();
            }

            MethodInfo[] methods = application.GetType().GetMethods();
            int agentAcitonIdIndex = 0;
            foreach (MethodInfo method in methods)
            {
                AgentAction agentAction = method.GetCustomAttribute<AgentAction>();
                if (agentAction == null) continue;
                _agentActionManager.AddAgentAction(agentActionIds[agentAcitonIdIndex], (context) => method.Invoke(application,new object[] { context }));
                agentAcitonIdIndex++;
            }
        }

        public void LoadApplications()
        {
            string[] directoryPaths = Directory.GetDirectories(AgentApplicationContext.ParentDirectoryPath);

            foreach (string directoryPath in directoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                string path = Directory.GetFiles(directoryPath, "*.dll")[0];
                AgentApplicationBase application = null;
                try
                {
                    application = GetApplicationImplementation(Assembly.LoadFile(path));
                }
                catch (Exception e) { }
                application.EventHandlerManager = _eventHandlerManager;

                using (var context = new AgentApplicationContext(directoryInfo.Name))
                {
                    application.Initialize();
                    _applications.Add(directoryInfo.Name, application);
                    _applicationResolver.ResolveAll(application);
                }
            }
        }

        public void InitializeApplication(string guid)
        {
            using (var context = new AgentApplicationContext(guid))
            {
                _applications[guid].Initialize();
            }
        }

        public void UninitializeApplication(string guid)
        {
            using (var context = new AgentApplicationContext(guid))
            {
                _applications[guid].Uninitialize();
            }
        }

        public List<object> GetApplicationsDescriptive()
        {
            List<object> applications = new List<object>();

            foreach(KeyValuePair<string, AgentApplicationBase> set in _applications)
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

        private AgentApplicationBase GetApplicationImplementation(Assembly assembly)
        {
            AgentApplicationBase app = null;
            Type applicationType = typeof(AgentApplicationBase);

            foreach (Type type in assembly.GetTypes())
            {
                if (applicationType.IsAssignableFrom(type))
                {
                    app = Activator.CreateInstance(type) as AgentApplicationBase;
                }
            }
                
            return app;
        }

    }
}
