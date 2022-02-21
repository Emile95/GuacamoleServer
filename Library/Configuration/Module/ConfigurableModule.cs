namespace Library.Configuration.Module
{
    public abstract class ConfigurableModule<Configuration> : IModule
    {
        public Configuration Config { get; set; }


        public abstract string GetModuleID();
        public abstract string GetModuleName();
        public abstract string GetModuleDescription();
    }
}
