namespace Library.Configuration.App.ConfigurationMember
{
    public class ConfigurationMember : Attribute
    {
        public string Label { get; set; }
        public object DefaultValue { get; set; }
    }
}