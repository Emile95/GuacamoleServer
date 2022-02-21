using System.Reflection;
using System.Text.Json;
using App.Exceptions;
using Library;
using Library.Logger;

namespace App
{
    public class ServerInstance
    {
        private readonly PluginLoader _pluginLoader;

        public List<IAction> Actions { get; set; }

        public List<IModule> Modules { get; set; }

        public ServerInstance()
        {
            _pluginLoader = new PluginLoader();
            Actions = new List<IAction>();
            Modules = new List<IModule>();
        }

        public void LoadPlugins()
        {
            Assembly[] assemblies = _pluginLoader.LoadDlls();

            foreach(Assembly assembly in assemblies)
            {
                Modules.AddRange(_pluginLoader.GetImplementations<IModule>(assembly));
                Actions.AddRange(_pluginLoader.GetImplementations<IAction>(assembly));
            }

            VerifyActionIDs();
            VerifyModuleIDs();
        }

        public object RunAction(IAction action, JsonElement runParameter)
        {
            ActionRunLogger logger = new ActionRunLogger(action);
            RunResult result = action.Run(runParameter, logger);
            if (result == null) return null;
            logger.Log("run succeded");
            return result.Result;
        }

        public object RunAction(IAction action)
        {
            ActionRunLogger logger = new ActionRunLogger(action);
            RunResult result = action.Run(logger);
            if (result == null) return null;
                logger.Log("run succeded");
            return result.Result;
        }

        private void VerifyActionIDs()
        {
            List<string> actionsIDs = new List<string>();
            for(int i = 0; i < Actions.Count; i++)
            {
                string actionID = Actions[i].GetActionID();
                if (actionsIDs.Contains(actionID))
                    throw new PluginIdException<IAction>(actionID);
                actionsIDs.Add(actionID);
            }
        }

        private void VerifyModuleIDs()
        {
            List<string> moduleIDs = new List<string>();
            for (int i = 0; i < Modules.Count; i++)
            {
                string moduleId = Modules[i].GetModuleID();
                if (moduleIDs.Contains(moduleId))
                    throw new PluginIdException<IModule>(moduleId);
                moduleIDs.Add(moduleId);
            }
        }
    }
}
