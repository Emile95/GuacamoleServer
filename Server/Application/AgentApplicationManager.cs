using API.Agent.Application;
using API.AgentAction.Configuration;
using Common;
using Server.AgentAction;
using System.Reflection;

namespace Server.Application
{
    public class AgentApplicationManager
    {
        private readonly Dictionary<string, AgentApplicationLoaded> _applications;
        private readonly AgentActionManager _serverAgentActionManager;

        public AgentApplicationManager(AgentActionManager serverAgentActionManager)
        {
            _applications = new Dictionary<string, AgentApplicationLoaded>();
            _serverAgentActionManager = serverAgentActionManager;
        }

        public void AddAgentApplication(string filePath, byte[] fileBin, List<string> agentActionIds)
        {
            string id = CreateNewId();
            AgentApplicationLoaded agentApplicationLoaded = new AgentApplicationLoaded
            {
                Id = id,
                ActionIds = agentActionIds,
                FileBinary = fileBin,
                FilePath = filePath
            };
            _applications.Add(id, agentApplicationLoaded);
        }

        public List<AgentApplicationLoaded> GetAgentApplicationLoadeds()
        {
            return _applications.Values.ToList();
        }

        public void LoadApplications()
        {
            string[] agentActionDirectoryPaths = Directory.GetDirectories(ApplicationConstValue.AGENTAPPSPATH);

            foreach (string agentActionDirectoryPath in agentActionDirectoryPaths)
            {
                string[] dlls = Directory.GetFiles(agentActionDirectoryPath, "*.dll");

                foreach (string dll in dlls)
                {
                    List<AgentApplicationBase> agentApplications = PluginFactory.CreatePluginsFromFile<AgentApplicationBase>(dll);

                    foreach (AgentApplicationBase agentApplication in agentApplications)
                    {
                        byte[] fileBinary = File.ReadAllBytes(dll);

                        List<string> agentActionIds = new List<string>();

                        MethodInfo[] methods = agentApplication.GetType().GetMethods();
                        foreach (MethodInfo method in methods)
                        {
                            AgentActionAttribute agentAction = method.GetCustomAttribute<AgentActionAttribute>();
                            if (agentAction == null) continue;
                            agentActionIds.Add(_serverAgentActionManager.AddAgentAction(agentAction));
                        }

                        AddAgentApplication(dll, fileBinary, agentActionIds);
                    }
                }
            }
        }

        private string CreateNewId()
        {
            return UniqueIdGenerator.Generate(_applications.Keys);
        }

        
    }
}
