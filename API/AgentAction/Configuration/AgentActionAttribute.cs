namespace API.AgentAction.Configuration
{ 
    public class AgentActionAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public Type ParameterType { get; set; }
    }
}
