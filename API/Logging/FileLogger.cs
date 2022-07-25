namespace API.Logging
{
    public class FileLogger : IAppLogger
    {
        public readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            Task.Run(() => {

                bool logDone = false;
                while(logDone)
                {
                    try
                    {
                        File.AppendAllText(_filePath, message + Environment.NewLine);
                        logDone = true;
                    }
                    catch (Exception e) { }
                }
            });
        }
    }
}
