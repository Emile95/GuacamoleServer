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

        public string PreviousGuid { get; private set; }
        public string PreviousAppPath { get; private set; }
        public List<HttpRequestAttribute> PreviousHttpRequestAttributes { get; set; } = new List<HttpRequestAttribute>();

        public ApplicationContext(string guid)
        {
            PreviousGuid = Guid;
            PreviousAppPath = AppPath;
            PreviousHttpRequestAttributes = HttpRequestAttributes;

            Guid = guid;
            AppPath = Path.Combine(ParentDirectoryPath, guid);
            HttpRequestAttributes = new List<HttpRequestAttribute>();
        }

        public void Dispose()
        {
            Guid = PreviousGuid;
            AppPath = PreviousAppPath;
            HttpRequestAttributes = PreviousHttpRequestAttributes;
        }
    }
}
