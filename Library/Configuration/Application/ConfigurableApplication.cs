using Library.Application;
using Library.Storage;

namespace Library.Configuration.App
{
    public abstract class ConfigurableApplication<Configuration> : ApplicationBase
        where Configuration : class, new()
    {
        protected Configuration Config { get; private set; }
        protected ConfigurationStorageManager<Configuration> ConfigurationStorageManager { get; private set; }

        private void UpdateConfiguration(Configuration newConfiguration)
        {
            Configuration oldConfiguration = Config;
            Config = newConfiguration;
            ConfigurationStorageManager.UpdateConfiguration(Config);
            OnConfigurationUpdate(oldConfiguration);
        }

        public override void Install() 
        {
            ConfigurationStorageManager = new ConfigurationStorageManager<Configuration>(ApplicationContext.AppPath);
            ConfigurationStorageManager.AddConfiguration(new Configuration());
        }

        public override void Initialize() 
        {
            ConfigurationStorageManager = new ConfigurationStorageManager<Configuration>(ApplicationContext.AppPath);
            Config = ConfigurationStorageManager.LoadConfiguration();
        }

        public override void Uninstall() 
        {
            ConfigurationStorageManager.RemoveConfiguration(Config);
        }

        public override void Uninitialize() 
        {
            ConfigurationStorageManager = null;
            Config = null;
        }

        protected sealed override void ValidateApplication()
        {
            ValidateConfiguration();
        }

        protected virtual void ValidateConfiguration() { }

        protected virtual void OnConfigurationUpdate(Configuration oldConfiguration) { }
    }
}
