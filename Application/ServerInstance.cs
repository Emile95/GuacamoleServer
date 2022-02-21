using System.Reflection;
using System.Text.Json;
using App.Exceptions;
using Library;
using Library.Configuration.Module.EventHandler;
using Library.Logger;

namespace App
{
    public class ServerInstance
    {
        private readonly PluginLoader _pluginLoader;

        public List<IAction> Actions { get; set; }

        public List<IModule> Modules { get; set; }

        public EventHandlerManager EventHandlerManager { get; set; }
        public ModuleResolver ModuleResolver { get; set; }

        public ServerInstance()
        {
            _pluginLoader = new PluginLoader();
            Actions = new List<IAction>();
            Modules = new List<IModule>();
            EventHandlerManager = new EventHandlerManager();
            ModuleResolver = new ModuleResolver();
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

            for(int i = 0; i < Modules.Count; i++)
            {
                object module = Modules[i];
                ModuleResolver.ResolveEventHandlers(EventHandlerManager, module);
            }
        }

        public object RunAction(IAction action, JsonElement runParameter)
        {
            return _RunAction(action, (act, logger) => act.Run(runParameter, logger));
        }

        public object RunAction(IAction action)
        {
            return _RunAction(action, (act, logger) => act.Run(logger));
        }

        private object _RunAction(IAction action, Func<IAction, ActionRunLogger, RunResult> func)
        {
            ActionRunLogger logger = new ActionRunLogger(action);
            EventHandlerContext context = new EventHandlerContext();
            EventHandlerManager.CallEventHandler(EventHandlerType.BeforeActionRun, context);
            RunResult result = func(action, logger);
            EventHandlerManager.CallEventHandler(EventHandlerType.AfterActionRun, context);
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
