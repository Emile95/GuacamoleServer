namespace API.Logging
{
    public abstract class AppLoggers
    {
        protected readonly List<IAppLogger> _loggers;

        public AppLoggers()
        {
            _loggers = InitialiseLoggers();
        }

        public void Log(string message)
        {
            foreach(IAppLogger logger in _loggers)
                logger.Log(message);
        }

        protected abstract List<IAppLogger> InitialiseLoggers();
    }
}
