using Library.Http;

namespace Library.Configuration.Http
{
    public class HttpRequestAttribute : Attribute
    {
        public HttpRequestType HttpRequestType { get; set; }
        public string Pattern { get; set; }
        public Type ExpectedBody { get; set; }
    }
}
