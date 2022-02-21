namespace Library.Configuration.Action.ConfigurationParameter
{
    public class ParameterNameAttribute : Attribute
    {
        public string Name { get; set; }

        public ParameterNameAttribute(string name)
        {
            Name = name;
        }
    }
}
