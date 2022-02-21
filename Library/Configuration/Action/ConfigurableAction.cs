using Library.Configuration.Action.ConfigurationParameter;
using Library.Logger;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json;

namespace Library.Configuration.Action
{
    public abstract class ConfigurableAction<Configuration> : ActionBase
        where Configuration : class
    {

        public override bool HasParameter()
        {
            return true;
        }

        public override void RunAction(RunResult runResult) { }

        public override RunResult Run(JsonElement runParameter, ActionRunLogger logger)
        {
            RunResult runResult = new RunResult();
            runResult.Result = "action '" + GetActionID() + "' succeed";
            try
            {
                Configuration config = GetConfigurationFromJsonElement(runParameter, logger);
                string messageNonValidConfig = "Config not valid";
                if (config == null)
                {
                    runResult.Result = messageNonValidConfig;
                    return runResult;
                }
                if (!ValidateConfiguration(config))
                {

                    logger.Log(messageNonValidConfig);
                    runResult.Result = messageNonValidConfig;
                    return runResult;
                }
                RunAction(config, runResult);
                logger.Log("run succeed");
                return runResult;
            } catch(Exception e)
            {
                runResult.Result = e.Message;
                return runResult;
            }
        }

        public abstract void RunAction(Configuration config, RunResult runResult);

        protected virtual bool ValidateConfiguration(Configuration config) { return true; }

        private Configuration GetConfigurationFromJsonElement(JsonElement runParameter, ActionRunLogger logger)
        {
            Configuration config = Activator.CreateInstance<Configuration>();

            Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(runParameter.ToString());

             if (dictionary == null) return config;

             PropertyInfo[] propertyInfos = config.GetType().GetProperties();
             foreach (PropertyInfo propertyInfo in propertyInfos)
             {
                string propertyName = propertyInfo.Name;
                ParameterNameAttribute parameterNameAttribute = propertyInfo.GetCustomAttribute<ParameterNameAttribute>();
                if(parameterNameAttribute != null)
                    propertyName = parameterNameAttribute.Name;
                
                if(!dictionary.ContainsKey(propertyName))
                {
                    ParameterRequiredAttribute parameterRequiredAttribute = propertyInfo.GetCustomAttribute<ParameterRequiredAttribute>();
                    if (parameterRequiredAttribute != null)
                    {
                        logger.Log("Config not valid");
                        throw new Exception(propertyName + " parameter is required");
                    }
                        
                }

                if (dictionary[propertyName].GetType().IsEquivalentTo(propertyInfo.PropertyType))
                    propertyInfo.SetValue(config, dictionary[propertyName]);
             }

            return config;
        }
    }
}
