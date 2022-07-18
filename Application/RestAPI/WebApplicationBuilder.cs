using Application.Agent;
using Application.DataModel;
using Application.Job;
using Library.Application;
using Microsoft.AspNetCore.Mvc;

namespace Application.RestAPI
{
    public static class WebApplicationBuilder
    {
        public static WebApplication BuildWebApplication(ApplicationManager applicationManager, AgentManager agentManager, JobStorage jobStorage)
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

            webApplication.MapPost("job/", ([FromBody] JobDefinition body) =>
            {
                jobStorage.AddJob(body);
            });

            webApplication.MapDelete("job/{jobName}", (string jobName) =>
            {
                jobStorage.RemoveJob(jobName);
            });

            webApplication.MapPost("job/run/", ([FromBody] StartJobDataModel body) =>
            {
                return agentManager.StartJobOnAgent(body);
            });

            return webApplication;
        }
    }
}
