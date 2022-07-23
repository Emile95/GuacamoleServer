using Common;
using Newtonsoft.Json;

namespace Agent.Config
{
    public static class AgentConfigResolver
    {
        public static AgentConfig ResolveConfig()
        {
            AgentConfig serverConfig = new AgentConfig();

            string file = File.ReadAllText(ApplicationConstValue.CONFIGFILEPATH);

            return JsonConvert.DeserializeObject<AgentConfig>(file);
        }
    }
}
