namespace API.Agent
{
    public class AgentDefinition
    {
        public string Name { get; set; }
        public List<string> Labels { get; set; }
        public int ConcurrentRun { get; set; }
    }
}
