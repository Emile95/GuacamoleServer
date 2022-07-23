namespace API.Agent.Configuration.Application.AgentAction
{
    public class AgentActionAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public Type ParameterType { get; set; }
    }
}
