using Library.Application;
using Library.Storage;

namespace Library.Configuration.App
{
    public abstract class ConfigurableApplication<Configuration> : ApplicationBase
        where Configuration : class
    {
        protected Configuration Config { get; private set; }
        protected ConfigurationStorageManager<Configuration> ConfigurationStorageManager { get; private set; }


        protected sealed override void ValidateApplication()
        {
            ValidateConfiguration();
        }

        public override void Install(ApplicationContext context) 
        {
            ConfigurationStorageManager = new ConfigurationStorageManager<Configuration>(context.Path);
            ConfigurationStorageManager.AddConfiguration(Config);
        }

        public override void Initialize(ApplicationContext context) 
        {
            ConfigurationStorageManager = new ConfigurationStorageManager<Configuration>(context.Path);
            ConfigurationStorageManager.LoadConfiguration(Config);
        }

        public override void Uninstall(ApplicationContext context) 
        {
            ConfigurationStorageManager.RemoveConfiguration(Config);
            Config = null;
            ConfigurationStorageManager = null;
            
        }

        public override void Unitialize(ApplicationContext context) 
        {
            Config = null;
            ConfigurationStorageManager = null;
        }

        protected virtual void ValidateConfiguration() { }
    }
}
