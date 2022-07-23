using Server.Agent;
using Server.Agent.Action;
using Server.DataModel;
using Microsoft.AspNetCore.Mvc;

namespace Server.RestAPI
{
    public static class WebApplicationBuilder
    {
        public static WebApplication BuildWebApplication(ServerApplicationManager applicationManager, AgentManager agentManager, ServerAgentActionManager agentActionManager)
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

            webApplication.MapPost("action/process", ([FromBody] ProcessActionDataModel processActionDataModel) =>
            {
                try
                {
                    agentActionManager.ProcessAgentAction(processActionDataModel);
                } catch (Exception ex)
                {
                    return ex.Message;
                }
                return "action started";
            });

            return webApplication;
        }
    }
}
