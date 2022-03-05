using Library.EventHandler;
using Library.Application;
using Library.Http;
using WebApp.RequestBody;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Reflection;

public class ServerInstance
{
    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;
    private readonly HttpRequestManager _httpRequestManager;

    public ServerInstance()
    {
        _eventHandlerManager = new EventHandlerManager();
        _httpRequestManager = new HttpRequestManager();
        _applicationResolver = new ApplicationResolver(_eventHandlerManager, _httpRequestManager);
        _applicationManager = new ApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );
        _applicationManager.LoadApplications();
    }

    public void RunHttpRequest(HttpRequestDefinition httpRequestDefinition, HttpRequestContext context)
    {
        try
        {
            EventHandlerContext eventHandlerContext = new EventHandlerContext
            {
                HttpRequestContext = context
            };
            _eventHandlerManager.CallEventHandlers(EventHandlerType.BeforeHttpRequest, eventHandlerContext);
            _httpRequestManager.RunHttpRequest(httpRequestDefinition, context);
            _eventHandlerManager.CallEventHandlers(EventHandlerType.AfterHttpRequest, eventHandlerContext);
        } catch(Exception ex)
        {
            context.ResponseBody = ex.Message;
        }
    }

    public void RunWebApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: "prelude",
                builder => builder.WithOrigins("*")
            );
        });

        var webApp = builder.Build();

        webApp.UseCors("prelude");

        webApp.MapGet("applications/", () =>
        {
            return _applicationManager.GetApplicationsDescriptive();
        });

        webApp.MapGet("applications/is-valid-guid/{guid}", (string guid) =>
        {
            return _applicationManager.IsValidGuid(guid);
        });

        webApp.MapPost("plugins/install/", ([FromBody] InstallPlugin body) =>
        {
            _applicationManager.InstallApplication(body.Path);
            return "plugin installed";
        });

        foreach (KeyValuePair<string, List<HttpRequestDefinition>> set in _httpRequestManager.HttpRequests)
        {
            foreach (HttpRequestDefinition httpRequestDefinition in set.Value)
            {
                switch (httpRequestDefinition.RequestType)
                {
                    case HttpRequestType.Post: webApp.MapPost(httpRequestDefinition.Pattern, (httpContext) => PrepareHttpRequest(set.Key, httpRequestDefinition, httpContext)); break;
                    case HttpRequestType.Get: webApp.MapGet(httpRequestDefinition.Pattern, (httpContext) => PrepareHttpRequest(set.Key, httpRequestDefinition, httpContext)); break;
                    case HttpRequestType.Put: webApp.MapPut(httpRequestDefinition.Pattern, (httpContext) => PrepareHttpRequest(set.Key, httpRequestDefinition, httpContext)); break;
                    case HttpRequestType.Delete: webApp.MapDelete(httpRequestDefinition.Pattern, (httpContext) => PrepareHttpRequest(set.Key, httpRequestDefinition, httpContext)); break;
                }
            }
        }

        webApp.Run();
    }
    private Task PrepareHttpRequest(string appGuid, HttpRequestDefinition requestDefinition, HttpContext httpContext)
    {
        return Task.Run(() =>
        {
            using var applicationContext = new ApplicationContext(appGuid);

            HttpRequestContext context = new HttpRequestContext();
            context.RequestType = requestDefinition.RequestType;

            foreach (KeyValuePair<string, object?> set in httpContext.Request.RouteValues)
                context.RouteDatas[set.Key] = set.Value;

            foreach (KeyValuePair<string, StringValues> set in httpContext.Request.Query)
                context.Parameters[set.Key] = set.Value.ToList();

            StreamReader reader = new StreamReader(httpContext.Request.Body);
            string requestBody = reader.ReadToEndAsync().Result;
            if (!string.IsNullOrEmpty(requestBody))
                context.RequestBody = requestBody;

            RunHttpRequest(requestDefinition, context);
            if (context.ResponseBody != null)
                httpContext.Response.WriteAsJsonAsync(context.ResponseBody);
        });
    }
}

