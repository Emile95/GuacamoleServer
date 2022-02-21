namespace Library
{
    public interface IRunnable<RunParameter, Logger>
        where Logger : ILogger
    {
        RunResult Run(RunParameter runParameter, Logger logger);
        RunResult Run(Logger logger);
    }
}
