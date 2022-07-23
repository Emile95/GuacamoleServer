﻿using Library;
using Library.Agent.Application;
using Library.Agent.Configuration.Application.AgentAction;
using Server.Agent.Action;
using System.Reflection;

namespace Server
{
    public class AgentApplicationManager
    {
        private readonly Dictionary<string, ServerAgentApplicationLoaded> _applications;
        private readonly ServerAgentActionManager _serverAgentActionManager;

        public AgentApplicationManager(ServerAgentActionManager serverAgentActionManager)
        {
            _applications = new Dictionary<string, ServerAgentApplicationLoaded>();
            _serverAgentActionManager = serverAgentActionManager;
        }

        public void AddAgentApplication(string filePath, byte[] fileBin, List<string> agentActionIds)
        {
            string id = CreateNewId();
            ServerAgentApplicationLoaded agentApplicationLoaded = new ServerAgentApplicationLoaded
            {
                Id = id,
                ActionIds = agentActionIds,
                FileBinary = fileBin,
                FilePath = filePath
            };
            _applications.Add(id, agentApplicationLoaded);
        }

        public List<ServerAgentApplicationLoaded> GetAgentApplicationLoadeds()
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
                            AgentAction agentAction = method.GetCustomAttribute<AgentAction>();
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
