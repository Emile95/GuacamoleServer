using Library.Configuration.Http;
using Newtonsoft.Json;
using System.Reflection;

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

        public void Add(Action<HttpRequestContext> action, IEnumerable<HttpRequestAttribute> httpRequestAttributes)
        {
            foreach (HttpRequestAttribute httpRequestAttribute in httpRequestAttributes)
            {
                HttpRequest[httpRequestAttribute.HttpRequestType].Add(new HttpRequestDefinition
                {
                    Pattern = httpRequestAttribute.Pattern,
                    Action = action,
                    ExpectedBody = httpRequestAttribute.ExpectedBody,
                });
            } 
        }

        public object RunHttpRequest(HttpRequestDefinition httpRequestDefinition, HttpRequestContext context)
        {
            if (context.RequestBody == null && httpRequestDefinition.ExpectedBody != null)
                throw new Exception("You need to provide a body to your request");
            if(context.RequestBody != null && httpRequestDefinition.ExpectedBody != null)
            {
                Dictionary<object, object> jsonObject = JsonConvert.DeserializeObject< Dictionary<object, object>>(context.RequestBody.ToString());
                context.RequestBody = CreateExpectedBody(httpRequestDefinition.ExpectedBody, jsonObject);
            }
            httpRequestDefinition.Action(context);
            return context.ResponseBody;
        }

        private object CreateExpectedBody(Type expectedBody, Dictionary<object, object> jsonObject)
        {
            object body = Activator.CreateInstance(expectedBody);

            PropertyInfo[] propertyInfos = expectedBody.GetProperties();
            foreach(PropertyInfo propertyInfo in propertyInfos)
            {
                BodyMemberAttribute attribute = propertyInfo.GetCustomAttribute<BodyMemberAttribute>();
                if (attribute == null) continue;

                string propertyName = string.IsNullOrEmpty(attribute.Name) ? propertyInfo.Name : attribute.Name;

                if(!jsonObject.ContainsKey(propertyName))
                {
                    if (attribute.IsRequired)
                        throw new Exception("Request body member '" + propertyName + "' is required");
                    continue;
                }

                object member = jsonObject[propertyName];

                if (!propertyInfo.PropertyType.Equals(member.GetType()))
                    throw new Exception("Request body member '" + propertyName + "' required a value of type '" + propertyInfo.PropertyType.Name + "'");

                propertyInfo.SetValue(body, member);
            }

            return body;
        }
    }
}
