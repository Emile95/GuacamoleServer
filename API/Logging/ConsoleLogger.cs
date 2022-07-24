namespace API.Logging
{
    public class ConsoleLogger : IAppLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
