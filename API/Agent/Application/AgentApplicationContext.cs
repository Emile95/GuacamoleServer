using Common;

namespace API.Agent.Application
{
    public class AgentApplicationContext : IDisposable
    {
        public static string ParentDirectoryPath { get; private set; } = ApplicationConstValue.SERVERAPPSPATH;
        public static string Guid { get; private set; }
        public static string AppPath { get; private set; }

        private string _previousGuid;
        public string _previousAppPath;

        public AgentApplicationContext(string guid)
        {
            _previousGuid = Guid;
            _previousAppPath = AppPath;

            Guid = guid;
            AppPath = Path.Combine(ParentDirectoryPath, guid);
        }

        public void Dispose()
        {
            Guid = _previousGuid;
            AppPath = _previousAppPath;
        }
    }
}
