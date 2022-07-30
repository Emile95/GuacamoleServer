using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agent
{
    internal static class Configuration
    {
        public static string AgentName { get; private set; }
        public static List<string> Labels { get; private set; }
        public static long ConcurrentRun { get; private set; }
        public static string ServerHost { get; private set; }
        public static long ServerPort { get; private set; }
        public static string ServerProtocol { get; private set; }

        public static void ResolveConfig()
        {
            string file = File.ReadAllText(ApplicationConstValue.CONFIGFILEPATH);

            JObject jobject;
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(file);

            AgentName = dict["name"] as string;

            Labels = (dict["labels"] as JArray).ToObject<List<string>>();
            ConcurrentRun = (long)dict["concurrentRun"];

            jobject = dict["serverSocketConfig"] as JObject;
            Dictionary<string, object> serverSocketConfig = jobject.ToObject<Dictionary<string, object>>();
            ServerHost = serverSocketConfig["host"] as string;
            ServerPort = (long)serverSocketConfig["port"];
            ServerProtocol = serverSocketConfig["protocol"] as string;
        }
    }
}
