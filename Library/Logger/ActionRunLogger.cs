using System.Reflection;

namespace Library.Logger
{
    public class ActionRunLogger : FileLogger
    { 
        private readonly IAction _action;

        public ActionRunLogger(IAction action)
        : base(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs", "action.log")) 
        {
            _action = action;
        }

        public override string GetLine(string message)
        {
            DateTime currentDate = DateTime.Now;
            return currentDate + " : action id = " + _action.GetActionID() + ", message = " + message;
        }
    }
}
