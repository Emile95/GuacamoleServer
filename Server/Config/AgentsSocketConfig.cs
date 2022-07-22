using System.Text.Json.Serialization;

namespace Server.Config
{
    [Serializable]
    public class AgentsSocketConfig
    {
        public string Protocol { get; set; }
        public int Port { get; set; }
    }
}
