using System.Text.Json.Serialization;

namespace Agent.Config
{
    public class ServerSocketConfig
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }
        [JsonPropertyName("port")]
        public int Port { get; set; }
    }
}
