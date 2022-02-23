using Library.Configuration.Http;

namespace Library.Http
{
    public class HttpRequestManager : IAttributeManager<Action<HttpRequestContext>, HttpRequestAttribute>
    {
        public Dictionary<HttpRequestType, List<HttpRequestDefinition>> HttpRequest { get; private set; }

        public HttpRequestManager()
        {
            HttpRequest = new Dictionary<HttpRequestType, List<HttpRequestDefinition>>();
            HttpRequest.Add(HttpRequestType.Get, new List<HttpRequestDefinition>());
            HttpRequest.Add(HttpRequestType.Put, new List<HttpRequestDefinition>());
            HttpRequest.Add(HttpRequestType.Post, new List<HttpRequestDefinition>());
            HttpRequest.Add(HttpRequestType.Delete, new List<HttpRequestDefinition>());
        }

        public void Add(Action<HttpRequestContext> httpRequest, IEnumerable<HttpRequestAttribute> httpRequestAttributes)
        {
            foreach (HttpRequestAttribute httpRequestAttribute in httpRequestAttributes)
                HttpRequest[httpRequestAttribute.HttpRequestType].Add(new HttpRequestDefinition { 
                    Pattern = httpRequestAttribute.Pattern,
                    Request = httpRequest,
                    ExcpectedType = httpRequestAttribute.ExpectedBody
                });
        }

        public object RunHttpRequest(Action<HttpRequestContext> httpRequest, HttpRequestContext context)
        {
            if(context.RequestBody != null && context.ExpectedBody != null)
                ValidateRequestBody(context.RequestBody);
            httpRequest(context);
            return context.ResponseBody;
        }

        private void ValidateRequestBody(object body)
        {

        }
    }
}
