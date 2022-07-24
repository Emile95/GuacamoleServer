using Common;

namespace API.Logging
{
    public class SocketLoggers : AppLoggers
    {
        protected override List<IAppLogger> InitialiseLoggers()
        {
            List<IAppLogger> loggers = new List<IAppLogger>();
            loggers.Add(new ConsoleLogger());
            loggers.Add(new FileLogger(ApplicationConstValue.SOCKETLOGFILEPATH));
            return loggers;
        }
    }
}
