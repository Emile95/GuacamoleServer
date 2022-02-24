using Library.Application;
using Library.Storage;
using Newtonsoft.Json;

namespace Library.Configuration.App
{
    public abstract class ConfigurableApplication<Configuration> : ApplicationBase
        where Configuration : class, new()
    {
        protected Configuration Config { get; private set; }
        protected ConfigurationStorageManager<Configuration> ConfigurationStorageManager { get; private set; }


        public ConfigurableApplication()
        {
            Config = new Configuration();
        }

        public void UpdateConfiguration(string configuration)
        {
            Configuration oldConfiguration = Config;
            Config = JsonConvert.DeserializeObject<Configuration>(configuration);
            ConfigurationStorageManager.UpdateConfiguration(Config);
            OnConfigurationUpdate(oldConfiguration);
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
            ConfigurationStorageManager = null;
            Config = null;
        }

        public override void Unitialize(ApplicationContext context) 
        {
            ConfigurationStorageManager = null;
            Config = null;
        }

        protected sealed override void ValidateApplication()
        {
            ValidateConfiguration();
        }

        protected virtual void ValidateConfiguration() { }

        protected virtual void OnConfigurationUpdate(Configuration oldConfiguration)
        {

        }
    }
}
