namespace Server.Logger
{
    public class ConsoleLogger : Library.Logger.ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
