using Application.Agent;
using Application.DataModel;
using Library.Application;
using Microsoft.AspNetCore.Mvc;

namespace Application.RestAPI
{
    public static class WebApplicationBuilder
    {
        public static WebApplication BuildWebApplication(ApplicationManager applicationManager, AgentManager agentManager)
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

            webApplication.MapPost("plugins/install/", ([FromBody] InstallPlugin body) =>
            {
                applicationManager.InstallApplication(body.Path);
                return "plugin installed";
            });

            webApplication.MapPost("job/run/", ([FromBody] JobRun body) =>
            {
                return agentManager.RunJobOnAgent(body);
            });

            return webApplication;
        }
    }
}
