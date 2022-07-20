using System.Text.Json.Serialization;

namespace GuacamoleAgent.Config
{
    public class ServerSocketConfig
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }
        [JsonPropertyName("port")]
        public int Port { get; set; }
    }
}
