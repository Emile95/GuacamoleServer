namespace Library.Logger
{
    public abstract class FileLogger : ILogger
    {
        protected string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message) 
        {
            File.AppendAllText(_filePath, GetLine(message) + "\n");
        }

        public abstract string GetLine(string message);
    }
}
