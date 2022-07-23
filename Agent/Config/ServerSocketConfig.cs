using System.Text.Json.Serialization;

namespace Agent.Config
{
    public class ServerSocketConfig
    {
        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }
        [JsonPropertyName("port")]
        public int Port { get; set; }
    }
}
