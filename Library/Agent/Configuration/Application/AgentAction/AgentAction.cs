namespace Library.Agent.Configuration.Application.AgentAction
{
    public class AgentAction : Attribute
    {
        public string DisplayName { get; set; }
        public Type ParameterType { get; set; }
    }
}
