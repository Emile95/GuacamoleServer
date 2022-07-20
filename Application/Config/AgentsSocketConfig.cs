using System.Text.Json.Serialization;

namespace Application.Config
{
    [Serializable]
    public class AgentsSocketConfig
    {
        public string Protocol { get; set; }
        public int Port { get; set; }
    }
}
