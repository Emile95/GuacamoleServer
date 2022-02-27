using Library.Configuration.Http;

namespace Library.Application
{
    public class ApplicationContext
    {
        public string Guid { get; set; }
        public string Path { get; set; }
        public List<HttpRequestAttribute> HttpRequestAttributes { get; set; }

        public ApplicationContext()
        {
            HttpRequestAttributes = new List<HttpRequestAttribute>();
        }
    }
}
