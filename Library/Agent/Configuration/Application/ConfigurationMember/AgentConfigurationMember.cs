namespace API.Agent.Configuration.Application.ConfigurationMember
{
    public class AgentConfigurationMember : Attribute
    {
        public string Label { get; set; }
        public object DefaultValue { get; set; }
    }
}