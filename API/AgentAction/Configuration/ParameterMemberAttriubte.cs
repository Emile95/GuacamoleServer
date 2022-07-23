namespace API.AgentAction.Configuration
{
    public class ParameterMemberAttriubte : Attribute
    {
        public string Label { get; set; }
        public bool IsRequired { get; set; }
        public object DefaultValue { get; set; }
    }
}
