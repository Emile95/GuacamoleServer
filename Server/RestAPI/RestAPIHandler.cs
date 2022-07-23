using Microsoft.AspNetCore.Mvc;

namespace Server.RestAPI
{
    public class RestAPIHandler
    {
        private readonly WebApplication _webApp;

        public RestAPIHandler()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddCors(options =>
             {
                 options.AddPolicy(
                     name: "prelude",
                     builder => builder.WithOrigins("*")
                 );
             });

            _webApp = builder.Build();
        }

        public void MapPost<Body>(string pattern, Func<Body,object> action)
        {
            _webApp.MapPost(pattern, ([FromBody] Body body) =>
            {
                try
                {
                    return action(body);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            });
        }

        public void Run()
        {
            _webApp.RunAsync();
        }

        public void Stop()
        {
            _webApp.StopAsync();
        }
    }
}
