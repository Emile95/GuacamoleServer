using Library.Configuration.Http;
using System.Reflection;

namespace Library.Application
{
    public class ApplicationContext : IDisposable
    {
        public static string ParentDirectoryPath { get; private set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App");
        public static string Guid { get; private set; }
        public static string AppPath { get; private set; }
        public static List<HttpRequestAttribute> HttpRequestAttributes { get; private set; } = new List<HttpRequestAttribute>();

        private string _previousGuid;
        public string _previousAppPath;
        private List<HttpRequestAttribute> _previousHttpRequestAttributes;

        public ApplicationContext(string guid)
        {
            _previousGuid = Guid;
            _previousAppPath = AppPath;
            _previousHttpRequestAttributes = HttpRequestAttributes;

            Guid = guid;
            AppPath = Path.Combine(ParentDirectoryPath, guid);
            HttpRequestAttributes = new List<HttpRequestAttribute>();
        }

        public void Dispose()
        {
            Guid = _previousGuid;
            AppPath = _previousAppPath;
            HttpRequestAttributes = _previousHttpRequestAttributes;
        }
    }
}
