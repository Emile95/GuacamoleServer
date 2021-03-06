using Application.Agent;
using Application.Agent.Action;
using Application.DataModel;
using Library.Application;
using Microsoft.AspNetCore.Mvc;

namespace Application.RestAPI
{
    public static class WebApplicationBuilder
    {
        public static WebApplication BuildWebApplication(ApplicationManager applicationManager, AgentManager agentManager, ServerAgentActionManager agentActionManager)
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "prelude",
                    builder => builder.WithOrigins("*")
                );
            });

            var webApplication = builder.Build();

            webApplication.UseCors("prelude");

            webApplication.MapGet("applications/", () =>
            {
                return applicationManager.GetApplicationsDescriptive();
            });

            webApplication.MapGet("applications/is-valid-guid/{guid}", (string guid) =>
            {
                return applicationManager.IsValidGuid(guid);
            });

            webApplication.MapPost("action/process", ([FromBody] ProcessActionDataModel processActionDataModel) =>
            {
                agentActionManager.ProcessAgentAction(processActionDataModel);
                return "hoho";
            });

            return webApplication;
        }
    }
}
