namespace API.Server.Application.ConfigurationMember
{
    public class ServerConfigurationMember : Attribute
    {
        public string Label { get; set; }
        public object DefaultValue { get; set; }
    }
}