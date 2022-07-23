using Common;
using Newtonsoft.Json;

namespace Server.Config
{
    public static class ServerConfigResolver
    {
        public static ServerConfig ResolveConfig()
        {
            string file = File.ReadAllText(ApplicationConstValue.CONFIGFILEPATH);

            ServerConfig serverConfig = null;

            try
            {
                serverConfig = JsonConvert.DeserializeObject<ServerConfig>(file);
            } catch(Exception e)
            {
                throw new Exception("config file is Invalid");
            }

            if (serverConfig == null)
                throw new Exception("config file is Invalid");

            return serverConfig;
        }
    }
}
