using App;
using Library;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var serverInstance = new ServerInstance();

serverInstance.LoadPlugins();

var builder = WebApplication.CreateBuilder(args);

var webApp = builder.Build();

webApp.MapGet("/plugins/modules", () =>
{
    return serverInstance.Modules.Count;
});

webApp.MapGet("/plugins/actions", () =>
{
    return serverInstance.Actions.Count;
});

foreach (IAction action in serverInstance.Actions)
{
    if(action.HasParameter())
    {
        webApp.MapPost("/actions/" + action.GetActionID(), ([FromBody] JsonElement runParameter) => {
            return serverInstance.RunAction(action, runParameter);
        });
        continue;
    }
    webApp.MapPost("/actions/" + action.GetActionID(), () => {
        return serverInstance.RunAction(action);
    });

}

webApp.Run();