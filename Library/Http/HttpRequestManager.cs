﻿using Library.Configuration.Http;
using Newtonsoft.Json;
using System.Reflection;

namespace Library.Http
{
    public class HttpRequestManager
    {
        public readonly Dictionary<HttpRequestType, List<HttpRequestDefinition>> HttpRequest;

        public HttpRequestManager()
        {
            HttpRequest = new Dictionary<HttpRequestType, List<HttpRequestDefinition>>();
            HttpRequest.Add(HttpRequestType.Get, new List<HttpRequestDefinition>());
            HttpRequest.Add(HttpRequestType.Put, new List<HttpRequestDefinition>());
            HttpRequest.Add(HttpRequestType.Post, new List<HttpRequestDefinition>());
            HttpRequest.Add(HttpRequestType.Delete, new List<HttpRequestDefinition>());
        }

        public void Add(Action<HttpRequestContext> action, HttpRequestAttribute httpRequestAttribute)
        {
            HttpRequest[httpRequestAttribute.HttpRequestType].Add(new HttpRequestDefinition
            {
                Pattern = httpRequestAttribute.Pattern,
                Action = action,
                ExpectedBody = httpRequestAttribute.ExpectedBody,
            });
        }

        public void RunHttpRequest(HttpRequestDefinition httpRequestDefinition, HttpRequestContext context)
        {
            if (context.RequestBody == null && httpRequestDefinition.ExpectedBody != null)
                throw new Exception("You need to provide a body to your request");
            if(context.RequestBody != null && httpRequestDefinition.ExpectedBody != null)
            {
                Dictionary<string, object> jsonObject = JsonConvert.DeserializeObject< Dictionary<string, object>>(context.RequestBody.ToString());
                context.RequestBody = CreateExpectedBody(httpRequestDefinition.ExpectedBody, jsonObject);
            }
            httpRequestDefinition.Action(context);
        }

        private object CreateExpectedBody(Type expectedBody, Dictionary<string, object> jsonObject)
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

                if (member != null && !propertyInfo.PropertyType.Equals(member.GetType()))
                    throw new Exception("Request body member '" + propertyName + "' required a value of type '" + propertyInfo.PropertyType.Name + "'");

                propertyInfo.SetValue(body, member);
            }

            return body;
        }
    }
}
