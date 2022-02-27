using Library.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using WebApp.RequestBody;

var serverInstance = new ServerInstance();

serverInstance.LoadApplications();

var builder = WebApplication.CreateBuilder(args);

var webApp = builder.Build();

Dictionary<HttpRequestType, List<HttpRequestDefinition>> httpRequests = serverInstance.GetHttpRequests();

webApp.MapGet("applications/", () =>
{
    return serverInstance.GetApplicationDescriptives();
});

webApp.MapPost("plugins/install/", ([FromBody] InstallPlugin body) =>
{
    return serverInstance.InstallPlugin(body.Path);
});

List<HttpRequestDefinition> getRequestDefinitions = httpRequests[HttpRequestType.Get];
for (int i = 0; i < getRequestDefinitions.Count; i++)
{
    HttpRequestDefinition requestDefinition = getRequestDefinitions[i];
    webApp.MapGet(requestDefinition.Pattern, (httpContext) =>
        PrepareHttpRequest(HttpRequestType.Get, requestDefinition, httpContext));
}

List<HttpRequestDefinition> postRequestDefinitions = httpRequests[HttpRequestType.Post];
for (int i = 0; i < postRequestDefinitions.Count; i++)
{
    HttpRequestDefinition requestDefinition = postRequestDefinitions[i];
    webApp.MapPost(requestDefinition.Pattern, (httpContext) =>
        PrepareHttpRequest(HttpRequestType.Post, requestDefinition, httpContext));
}

List<HttpRequestDefinition> putRequestDefinitions = httpRequests[HttpRequestType.Put];
for (int i = 0; i < putRequestDefinitions.Count; i++)
{
    HttpRequestDefinition requestDefinition = putRequestDefinitions[i];
    webApp.MapPut(requestDefinition.Pattern, (httpContext) =>
        PrepareHttpRequest(HttpRequestType.Put, requestDefinition, httpContext));
}

List<HttpRequestDefinition> deleteRequestDefinitions = httpRequests[HttpRequestType.Delete];
for (int i = 0; i < deleteRequestDefinitions.Count; i++)
{
    HttpRequestDefinition requestDefinition = deleteRequestDefinitions[i];
    webApp.MapDelete(requestDefinition.Pattern, (httpContext) =>
       PrepareHttpRequest(HttpRequestType.Delete, requestDefinition, httpContext));
    
}

Task PrepareHttpRequest(HttpRequestType httpRequestType, HttpRequestDefinition requestDefinition, HttpContext httpContext)
{
    return Task.Run(() =>
    {
        HttpRequestContext context = new HttpRequestContext();
        context.RequestType = httpRequestType;

        foreach(KeyValuePair<string,object?> set in httpContext.Request.RouteValues)
            context.RouteDatas[set.Key] = set.Value;

        foreach(KeyValuePair<string, StringValues> set in httpContext.Request.Query)
            context.Parameters[set.Key] = set.Value.ToList();
        
        StreamReader reader = new StreamReader(httpContext.Request.Body);
        string requestBody = reader.ReadToEndAsync().Result;
        if (!string.IsNullOrEmpty(requestBody))
            context.RequestBody = requestBody;

        serverInstance.RunHttpRequest(requestDefinition, context);
        if(context.ResponseBody != null)
            httpContext.Response.WriteAsJsonAsync(context.ResponseBody);
    });
}

webApp.Run();