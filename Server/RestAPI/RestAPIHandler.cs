using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;

namespace Server.RestAPI
{
    public class RestAPIHandler
    {
        private readonly API.Logger.ILogger _logger;
        private readonly WebApplication _webApp;

        public RestAPIHandler(API.Logger.ILogger logger)
        {
            _logger = logger;

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
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string ipStr = "http://"+ ip.ToString() + ":5000";
                    _webApp.RunAsync(ipStr);
                }
            }
        }

        public void Stop()
        {
            _webApp.StopAsync();
        }
    }
}
