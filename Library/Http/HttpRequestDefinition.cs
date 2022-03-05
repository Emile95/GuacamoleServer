using System.Reflection;

namespace Library.Http
{
    public class HttpRequestDefinition
    {
        public string Pattern { get; set; }
        public Action<HttpRequestContext> Action { get; set; }
        public Type ExpectedBody { get; set; }
        public HttpRequestType RequestType { get; set; }
    }
}
