using Library;

namespace Server
{
    public class AgentApplicationManager
    {
        private readonly Dictionary<string, ServerAgentApplicationLoaded> _applications;

        public AgentApplicationManager()
        {
            _applications = new Dictionary<string, ServerAgentApplicationLoaded>();
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

        private string CreateNewId()
        {
            return UniqueIdGenerator.Generate(_applications.Keys);
        }

        
    }
}
