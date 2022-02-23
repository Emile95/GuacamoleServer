using App;
using Library.Http;

var serverInstance = new ServerInstance();

serverInstance.LoadPlugins();

var builder = WebApplication.CreateBuilder(args);

var webApp = builder.Build();

Dictionary<HttpRequestType, List<Library.Http.HttpRequestDefinition>> httpRequests = serverInstance.GetHttpRequests();

List<Library.Http.HttpRequestDefinition> getRequestDefinitions = httpRequests[HttpRequestType.Get];
for (int i = 0; i < getRequestDefinitions.Count; i++)
{
    Library.Http.HttpRequestDefinition requestDefinition = getRequestDefinitions[i];
    webApp.MapPost(requestDefinition.Pattern, (httpContext) =>
        Task.Run(() => {
            HttpRequestContext context = new HttpRequestContext();
            context.RequestType = HttpRequestType.Get;

            object obj = serverInstance.RunHttpRequest(requestDefinition, context);
            httpContext.Response.WriteAsJsonAsync(obj);
        })
    );
}

List<Library.Http.HttpRequestDefinition> postRequestDefinitions = httpRequests[HttpRequestType.Post];
for (int i = 0; i < postRequestDefinitions.Count; i++)
{
    Library.Http.HttpRequestDefinition requestDefinition = postRequestDefinitions[i];
    webApp.MapPost(requestDefinition.Pattern, (httpContext) => 
        Task.Run(() => {
            HttpRequestContext context = new HttpRequestContext();
            context.RequestType = HttpRequestType.Post;

            StreamReader reader = new StreamReader(httpContext.Request.Body);
            context.RequestBody = reader.ReadToEndAsync().Result;

            object obj = serverInstance.RunHttpRequest(requestDefinition, context);
            httpContext.Response.WriteAsJsonAsync(obj);
        })
    );
}

List<Library.Http.HttpRequestDefinition> putRequestDefinitions = httpRequests[HttpRequestType.Put];
for (int i = 0; i < putRequestDefinitions.Count; i++)
{
    Library.Http.HttpRequestDefinition requestDefinition = putRequestDefinitions[i];
    webApp.MapPut(requestDefinition.Pattern, (httpContext) =>
        Task.Run(() => {
            HttpRequestContext context = new HttpRequestContext();
            context.RequestType = HttpRequestType.Put;

            StreamReader reader = new StreamReader(httpContext.Request.Body);
            context.RequestBody = reader.ReadToEndAsync().Result;

            object obj = serverInstance.RunHttpRequest(requestDefinition, context);
            httpContext.Response.WriteAsJsonAsync(obj);

        })
    );
}

List<Library.Http.HttpRequestDefinition> deleteRequestDefinitions = httpRequests[HttpRequestType.Delete];
for (int i = 0; i < deleteRequestDefinitions.Count; i++)
{
    Library.Http.HttpRequestDefinition requestDefinition = deleteRequestDefinitions[i];
    webApp.MapDelete(requestDefinition.Pattern, (httpContext) =>
       Task.Run(() => {
           HttpRequestContext context = new HttpRequestContext();
           context.RequestType = HttpRequestType.Delete;

           StreamReader reader = new StreamReader(httpContext.Request.Body);
           context.RequestBody = reader.ReadToEndAsync().Result;

           object obj = serverInstance.RunHttpRequest(requestDefinition, context);
           httpContext.Response.WriteAsJsonAsync(obj);

       })
   );
}

webApp.Run();