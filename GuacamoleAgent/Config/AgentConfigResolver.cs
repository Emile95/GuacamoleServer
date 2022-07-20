using Library;
using Newtonsoft.Json;

namespace GuacamoleAgent.Config
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
