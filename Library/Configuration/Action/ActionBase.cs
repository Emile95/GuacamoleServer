using Library.Logger;
using System.Text.Json;

namespace Library.Configuration.Action
{
    public abstract class ActionBase : IAction
    {
        public virtual bool HasParameter()
        {
            return false;
        }

        public RunResult Run(ActionRunLogger logger)
        {
            RunResult runResult = new RunResult();
            runResult.Result = "action '" + GetActionID() + "' succeed";
            try
            {
                RunAction(runResult);
            }
            catch (Exception e)
            {
                runResult.Result = e.Message;
            }
            logger.Log("run succeed");
            return runResult;
        }

        public virtual RunResult Run(JsonElement runParameter, ActionRunLogger logger) { return null; }

        public abstract string GetActionID();

        public abstract void RunAction(RunResult runResult);
    }
}
