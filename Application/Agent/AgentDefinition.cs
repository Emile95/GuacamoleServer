using System.Text.Json.Serialization;

namespace Application.Agent
{
    public class AgentDefinition
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}
