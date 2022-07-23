namespace Server.Logger
{
    public class ConsoleLogger : API.Logger.ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
