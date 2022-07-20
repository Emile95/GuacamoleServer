using System.Text.Json.Serialization;

namespace GuacamoleAgent.ServerApplication
{
    [Serializable]
    public class AgentDefinition
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }
        [JsonPropertyName("concurrentRun")]
        public int ConcurrentRun { get; set; }
    }
}
