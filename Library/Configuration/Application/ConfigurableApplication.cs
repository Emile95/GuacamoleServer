using Library.Application;
using Library.Configuration.Http;
using Library.Http;
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

        public override void Install(ApplicationContext context) 
        {
            ConfigurationStorageManager = new ConfigurationStorageManager<Configuration>(context.Path);
            ConfigurationStorageManager.AddConfiguration(new Configuration());
        }

        public override void Initialize(ApplicationContext context) 
        {
            ConfigurationStorageManager = new ConfigurationStorageManager<Configuration>(context.Path);
            Config = ConfigurationStorageManager.LoadConfiguration();
        }

        public override void Uninstall(ApplicationContext context) 
        {
            ConfigurationStorageManager.RemoveConfiguration(Config);
        }

        public override void Uninitialize(ApplicationContext context) 
        {
            ConfigurationStorageManager = null;
            Config = null;
        }

        protected sealed override void ValidateApplication(ApplicationContext context)
        {
            ValidateConfiguration(context);
        }

        protected virtual void ValidateConfiguration(ApplicationContext context) { }

        protected virtual void OnConfigurationUpdate(Configuration oldConfiguration) { }

        public sealed override void AddBaseHttpRequests(ApplicationContext applicationContext, HttpRequestManager httpRequestManager) 
        {
            httpRequestManager.Add((context) => 
            {
                UpdateConfiguration(context.RequestBody as Configuration);
                context.ResponseBody = "Configuration updated";
            }, new HttpRequestAttribute() 
            {
                HttpRequestType = HttpRequestType.Post,
                Pattern = applicationContext.Guid + "/config",
                ExpectedBody = typeof(Configuration)
            });

            httpRequestManager.Add((context) => 
            {
                context.ResponseBody = Config;
            }, new HttpRequestAttribute()
            {
                HttpRequestType = HttpRequestType.Get,
                Pattern = applicationContext.Guid + "/config"
            });
        }
    }
}
