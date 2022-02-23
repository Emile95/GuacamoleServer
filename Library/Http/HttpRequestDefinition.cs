namespace Library.Http
{
    public class HttpRequestDefinition
    {
        public string Pattern { get; set; }
        public Action<HttpRequestContext> Request { get; set; }
        public Type ExcpectedType { get; set; }
    }
}
