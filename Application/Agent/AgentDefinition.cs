using System.Text.Json.Serialization;

namespace Application.Agent
{
    public class AgentDefinition
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }
    }
}
