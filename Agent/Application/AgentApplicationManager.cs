using Agent.AgentAction;
using API.Agent.Application;
using API.Agent.EventHandler;
using API.AgentAction.Configuration;
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
            List<AgentApplicationBase> applications = GetApplicationImplementation(Assembly.LoadFile(newApplicationPath));
            int agentAcitonIdIndex = 0;
            foreach (AgentApplicationBase application in applications)
            {
                application.EventHandlerManager = _eventHandlerManager;
                using (var context = new AgentApplicationContext(newGuid))
                {
                    application.Install();
                }
                MethodInfo[] methods = application.GetType().GetMethods();
                
                foreach (MethodInfo method in methods)
                {
                    AgentActionAttribute agentAction = method.GetCustomAttribute<AgentActionAttribute>();
                    if (agentAction == null) continue;
                    _agentActionManager.AddAgentAction(agentActionIds[agentAcitonIdIndex], (context) => method.Invoke(application, new object[] { context }));
                    agentAcitonIdIndex++;
                }
            }
        }

        public void LoadApplications()
        {
            string[] directoryPaths = Directory.GetDirectories(AgentApplicationContext.ParentDirectoryPath);

            foreach (string directoryPath in directoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                string path = Directory.GetFiles(directoryPath, "*.dll")[0];
                List<AgentApplicationBase> applications = null;
                try
                {
                    applications = GetApplicationImplementation(Assembly.LoadFile(path));
                }
                catch (Exception e) { }
                foreach (AgentApplicationBase application in applications)
                {
                    application.EventHandlerManager = _eventHandlerManager;

                    using (var context = new AgentApplicationContext(directoryInfo.Name))
                    {
                        application.Initialize();
                        _applications.Add(directoryInfo.Name, application);
                        _applicationResolver.ResolveAll(application);
                    }
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

        private List<AgentApplicationBase> GetApplicationImplementation(Assembly assembly)
        {
            List<AgentApplicationBase> apps = new List<AgentApplicationBase>();
            Type applicationType = typeof(AgentApplicationBase);

            foreach (Type type in assembly.GetTypes())
            {
                if (applicationType.IsAssignableFrom(type))
                {
                    apps.Add(Activator.CreateInstance(type) as AgentApplicationBase);
                }
            }
                
            return apps;
        }
    }
}
