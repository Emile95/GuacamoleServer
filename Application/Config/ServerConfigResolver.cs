using Library;
using Newtonsoft.Json;

namespace Application.Config
{
    public static class ServerConfigResolver
    {
        public static ServerConfig ResolveConfig()
        {
            ServerConfig serverConfig = new ServerConfig();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            string file = File.ReadAllText(ApplicationConstValue.CONFIGFILEPATH);

            return JsonConvert.DeserializeObject<ServerConfig>(file);
        }
    }
}
