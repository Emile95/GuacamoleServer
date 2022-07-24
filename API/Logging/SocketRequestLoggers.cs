using Common;

namespace API.Logging
{
    public class SocketRequestLoggers : AppLoggers
    {
        protected override List<IAppLogger> InitialiseLoggers()
        {
            List<IAppLogger> loggers = new List<IAppLogger>();
            loggers.Add(new ConsoleLogger());
            loggers.Add(new FileLogger(ApplicationConstValue.SOCKETREQUESTLOGFILEPATH));
            return loggers;
        }
    }
}
