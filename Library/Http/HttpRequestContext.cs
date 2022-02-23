namespace Library.Http
{
    public class HttpRequestContext
    {
        public object RequestBody { get; set; }
        public object ResponseBody { get; set; }
        public HttpRequestType RequestType { get; set; }
    }
}
