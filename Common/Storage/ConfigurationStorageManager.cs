using Newtonsoft.Json;

namespace Common.Storage
{
    public class ConfigurationStorageManager<Configuration>
    {
        private readonly string _configurationFilePath;

        public ConfigurationStorageManager(string appPath)
        {
            _configurationFilePath = Path.Combine(appPath, "config.json");
           
        }

        public Configuration LoadConfiguration()
        {
            string json = File.ReadAllText(_configurationFilePath);
            return JsonConvert.DeserializeObject<Configuration>(json);
        }

        public void UpdateConfiguration(Configuration updatedConfiguration)
        {
            File.Delete(_configurationFilePath);
            File.WriteAllText(_configurationFilePath, JsonConvert.SerializeObject(updatedConfiguration));
        }

        public void AddConfiguration(Configuration newConfiguration)
        {
            File.WriteAllText(_configurationFilePath, JsonConvert.SerializeObject(newConfiguration));
        }

        public void RemoveConfiguration(Configuration configuration)
        {
            File.Delete(_configurationFilePath);
        }
    }
}
