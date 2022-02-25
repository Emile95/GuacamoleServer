namespace Library.Http
{
    public class HttpRequestContext
    {
        public object RequestBody { get; set; }
        public object ResponseBody { get; set; }
        public HttpRequestType RequestType { get; set; }
        public Dictionary<string, object> RouteDatas { get; set; }
        public Dictionary<string, List<string>> Parameters { get; set; }

        public HttpRequestContext()
        {
            RouteDatas = new Dictionary<string, object>();
            Parameters = new Dictionary<string, List<string>>();
        }

    }
}
