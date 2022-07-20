using System.Text.Json.Serialization;

namespace GuacamoleAgent.Config
{
    public class AgentConfig
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }
        [JsonPropertyName("concurrentRun")]
        public int ConcurrentRun { get; set; }
        [JsonPropertyName("serverSocketConfig")]
        public ServerSocketConfig ServerSocketConfig { get; set; }
    }
}
